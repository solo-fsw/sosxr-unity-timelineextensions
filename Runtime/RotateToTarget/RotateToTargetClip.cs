using System;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the RotateToTarget track. Configure which axes to rotate on, the slerp ease speed, and the Transform to rotate (resolved via <see cref="ExposedReference{T}"/>). The track binding is the rotation target.
    /// </summary>
    [Serializable]
    public class RotateToTargetClip : Clip
    {
        [Tooltip("Which axis to use for calculations? 0 = don't use, 1 = use")]
        public Vector3Int AxisToUse = new(1, 0, 1);

        [SerializeField]
        [Range(0.001f, 10f)]
        private float m_easeSpeed = 1f;
        public ExposedReference<Transform> Rotator;

        [HideInInspector]
        public RotateToTargetBehaviour Template = new();

        public float EaseSpeed
        {
            get => m_easeSpeed;
            set => m_easeSpeed = value;
        }

        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<RotateToTargetBehaviour> playable =
                ScriptPlayable<RotateToTargetBehaviour>.Create(graph, Template); // Create a playable using the constructor
            var clone = playable.GetBehaviour(); // Get behaviour

            clone.InitializeBehaviour(TimelineClip, TrackBinding);
            clone.Rotator = Rotator.Resolve(Resolver);
            clone.EaseSpeed = EaseSpeed;
            clone.AxisToUse = AxisToUse;

            return playable;
        }
    }
}
