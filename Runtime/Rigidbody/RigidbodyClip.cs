using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the Rigidbody track. Configure isKinematic, useGravity, and an optional addForce impulse toward a
    ///     target Transform. All values are applied to the bound <see cref="Rigidbody"/> when the clip starts.
    /// </summary>
    [Serializable]
    public class RigidbodyClip : Clip
    {
        public bool IsKinematic;
        public bool UseGravity;
        public bool AddForce;
        public ExposedReference<Transform> Target;
        public float Amount;
        public ForceMode ForceMode;

        [HideInInspector] public RigidbodyBehaviour Template;

        public override ClipCaps clipCaps => ClipCaps.Blending;

        /// <summary>
        ///     Creates the RigidbodyBehaviour playable for this clip and wires up data.
        /// </summary>
        /// <param name="graph">PlayableGraph to which the playable belongs.</param>
        /// <param name="owner">Owner GameObject (usually the clip's host).</param>
        /// <returns>A ScriptPlayable of RigidbodyBehaviour configured for this clip.</returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<RigidbodyBehaviour> playable = ScriptPlayable<RigidbodyBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();

            clone.IsKinematic = IsKinematic;
            clone.UseGravity = UseGravity;
            clone.AddForce = AddForce;
            clone.Amount = Amount;
            
            // Validate and resolve ExposedReference
            var resolvedTarget = Target.Resolve(Resolver);
            if (resolvedTarget == null && AddForce)
            {
                Debug.LogWarning($"{TypeName}: Force target is assigned but could not be resolved. Make sure the target Transform is assigned in the clip.");
            }
            clone.Target = resolvedTarget;
            clone.ForceMode = ForceMode;

            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            return playable;
        }
    }
}
