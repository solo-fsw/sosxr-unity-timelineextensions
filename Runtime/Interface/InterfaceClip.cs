using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class InterfaceClip : PlayableAsset, ITimelineClipAsset
    {
        public InterfaceBehaviour Template;

        public TimelineClip TimelineClip { get; set; }

        public ClipCaps clipCaps => ClipCaps.None; // Do not allow blending between clips


        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<InterfaceBehaviour>.Create(graph, Template); // Create a playable, using the constructor

            Template = playable.GetBehaviour(); // Set it directly to the behaviour

            SetDisplayName(Template, TimelineClip);

            return playable;
        }


        private void SetDisplayName(InterfaceBehaviour template, TimelineClip timelineClip)
        {
            var displayName = "";

            if (template.Interface != null)
            {
                var typeName = template.Interface.GetType().Name;
                displayName = "Bound to " + typeName + " on: " + template.InterfaceObject.name;
            }

            displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Interface Clip");

            if (timelineClip == null)
            {
                return;
            }

            timelineClip.displayName = displayName;
        }
    }
}