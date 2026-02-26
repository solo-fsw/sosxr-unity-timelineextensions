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

        public override ClipCaps clipCaps => ClipCaps.None;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<RigidbodyBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();

            clone.isKinematic = IsKinematic;
            clone.useGravity = UseGravity;
            clone.addForce = AddForce;
            clone.amount = Amount;
            clone.target = Target.Resolve(Resolver);
            clone.forceMode = ForceMode;

            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            return playable;
        }
    }
}