using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class ParentingClip : Clip
    {
        [SerializeField] private ExposedReference<Transform> m_child; // See: https://docs.unity3d.com/ScriptReference/ExposedReference_1.html
        [SerializeField] public bool m_zeroInOnParent;
        [HideInInspector] public ParentingBehaviour Template = new();

        public override ClipCaps clipCaps => ClipCaps.None; // No blend


        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Template.ZeroInOnParent = m_zeroInOnParent;

            var playable = ScriptPlayable<ParentingBehaviour>.Create(graph, Template); // Create a playable, using the constructor

            var clone = playable.GetBehaviour(); // Get behaviour

            clone.InitializeBehaviour(TimelineClip, TrackBinding);
            var child = m_child.Resolve(Resolver);

            if (child != null)
            {
                clone.Child = child;
                clone.OriginalParent = child.parent;
            }

            return playable;
        }


        public override void InitializeClip(object trackBinding, TimelineClip timelineClip, IExposedPropertyTable resolver)
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);
            TimelineClip.displayName = "Parenting: " + (m_child.Resolve(resolver)?.name ?? "Unknown GameObject");
        }
    }
}