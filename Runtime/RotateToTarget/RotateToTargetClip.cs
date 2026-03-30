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

        public ExposedReference<Transform> Rotator;

        [HideInInspector]
        public RotateToTargetBehaviour Template = new();

        /// <summary>
        ///     Creates the RotateToTarget playable for this clip and wires up data.
        /// </summary>
        /// <param name="graph">PlayableGraph to which the playable belongs.</param>
        /// <param name="owner">Owner GameObject (usually the clip's host).</param>
        /// <returns>A ScriptPlayable of RotateToTargetBehaviour configured for this clip.</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<RotateToTargetBehaviour> playable =
                ScriptPlayable<RotateToTargetBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();

            clone.InitializeBehaviour(TimelineClip, TrackBinding);
            
            // Validate and resolve ExposedReference
            var resolvedRotator = Rotator.Resolve(Resolver);
            if (resolvedRotator == null)
            {
                Debug.LogWarning($"{GetType().Name}: Rotator could not be resolved. Make sure the rotator Transform is assigned in the clip.");
            }
            clone.Rotator = resolvedRotator;
            clone.AxisToUse = AxisToUse;

            return playable;
        }
    }
}
