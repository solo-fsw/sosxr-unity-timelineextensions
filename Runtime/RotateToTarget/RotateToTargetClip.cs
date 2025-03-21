using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class RotateToTargetClip : Clip
    {
        [Tooltip("Which axis to use for calculations? 0 = don't use, 1 = use")]
        public Vector3Int AxisToUse = new(1, 0, 1);
        [Range(0.001f, 10f)] public float EaseSpeed = 1f;
        public ExposedReference<Transform> Rotator;

        [HideInInspector] public RotateToTargetBehaviour Template = new();


        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<RotateToTargetBehaviour>.Create(graph, Template); // Create a playable using the constructor
            var clone = playable.GetBehaviour(); // Get behaviour

            clone.InitializeBehaviour(TimelineClip, TrackBinding);
            clone.Rotator = Rotator.Resolve(Resolver);
            clone.EaseSpeed = EaseSpeed;
            clone.AxisToUse = AxisToUse;

            return playable;
        }
    }
}