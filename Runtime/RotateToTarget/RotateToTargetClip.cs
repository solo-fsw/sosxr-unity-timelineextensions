using System;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the RotateToTarget track. Configure which axes to rotate on, and the Transform to look at (<see cref="Rotator"/>, resolved via <see cref="ExposedReference{T}"/>). The track binding is the Transform that rotates.
    /// </summary>
    [Serializable]
    public class RotateToTargetClip : Clip
    {
        /// <summary>Axes used for the look direction. 0 ignores that axis, 1 uses it.</summary>
        [Tooltip("Which axis to use for calculations? 0 = don't use, 1 = use")]
        public Vector3Int AxisToUse = new(1, 0, 1);

        /// <summary>The Transform to look at. Resolved through an <see cref="ExposedReference{T}"/> when the playable is created.</summary>
        [Tooltip("The Transform to look at (the target). The Transform bound to the track is the one that rotates.")]
        public ExposedReference<Transform> Rotator;

        /// <summary>Behaviour template passed to the playable created in <c>CreatePlayable</c>. Hidden in the Inspector.</summary>
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
                Debug.LogWarning($"{TypeName}: Rotator could not be resolved. Make sure the rotator Transform is assigned in the clip.");
            }
            clone.Rotator = resolvedRotator;
            clone.AxisToUse = AxisToUse;

            return playable;
        }
    }
}
