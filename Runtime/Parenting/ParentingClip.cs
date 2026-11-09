using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the Parenting track. References the child Transform to reparent via an <see cref="ExposedReference{T}"/>
    ///     so it can reference scene objects. Optionally zeros out the child's local pose on attachment.
    /// </summary>
    [Serializable]
    public class ParentingClip : Clip
    {
        [SerializeField]
        private ExposedReference<Transform> m_child; // See: https://docs.unity3d.com/ScriptReference/ExposedReference_1.html

        [SerializeField]
        public KeepPosition m_keepPosition;

        [HideInInspector]
        public ParentingBehaviour Template = new();

        public override ClipCaps clipCaps => ClipCaps.None; // No blend

        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Template.KeepPosition = m_keepPosition;

            ScriptPlayable<ParentingBehaviour> playable = ScriptPlayable<ParentingBehaviour>.Create(
                graph,
                Template
            );

            var clone = playable.GetBehaviour(); // Get behaviour
            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            var child = m_child.Resolve(Resolver);

            if (child != null)
            {
                clone.Child = child;
                clone.OriginalParent = child.parent; // Re-captured at clip start
            }

            return playable;
        }

        public override void InitializeClip(
            object trackBinding,
            TimelineClip timelineClip,
            IExposedPropertyTable resolver
        )
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);

            TimelineClip.displayName =
                "Parenting: " + (m_child.Resolve(resolver)?.name ?? "Unknown GameObject");
        }
    }

    /// <summary>How the child is placed when it gets parented. Values are serialized by index: only append new ones.</summary>
    public enum KeepPosition
    {
        /// <summary>Legacy name. Same as <see cref="KeepWorldPosition"/>.</summary>
        [InspectorName("Yes (Keep World Position)")]
        Yes,

        /// <summary>Child stays where it is in the world.</summary>
        KeepWorldPosition,

        /// <summary>Child moves to the parent; local position and rotation are set to zero / identity.</summary>
        ZeroOutCompletely,

        /// <summary>Child keeps its local position and rotation values, now relative to the new parent.</summary>
        KeepLocalPosition,
    }
}
