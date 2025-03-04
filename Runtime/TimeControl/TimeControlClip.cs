using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class TimeControlClip : PlayableAsset, ITimelineClipAsset
    {
        public TimeControlBehaviour Template;

        public TimelineClip TimelineClip { get; set; }


        public ClipCaps clipCaps => ClipCaps.None; // Do not allow blending between clips


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimeControlBehaviour>.Create(graph, Template);

            Template = playable.GetBehaviour(); // Set it directly to the behaviour
            Template.TimeControlClip = this;
            SetDisplayName(Template, TimelineClip);

            return playable;
        }


        /// <summary>
        ///     The displayName of the clip in Timeline will be set using this method.
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        public void SetDisplayName(TimeControlBehaviour timeControlBehaviour, TimelineClip clip)
        {
            var displayName = "";

            if (timeControlBehaviour.CurrentState == TimeState.TimeScaleZero)
            {
                displayName = "|| pausing";
            }
            else if (timeControlBehaviour.CurrentState == TimeState.Continue)
            {
                displayName = "● do not loop";
            }
            else if (timeControlBehaviour.CurrentState == TimeState.Looping)
            {
                displayName = "↩︎ loop clip";
            }
            else if (timeControlBehaviour.CurrentState == TimeState.GoToStart)
            {
                displayName = "← go to clip start";
            }
            else if (timeControlBehaviour.CurrentState == TimeState.GoToEnd)
            {
                displayName = "→ go to clip end";
            }

            if (timeControlBehaviour.TimeControlBase != null)
            {
                displayName += " || Time Controller: " + timeControlBehaviour.TimeControlBase.gameObject.name;
            }

            displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Looper Clip");

            if (clip == null)
            {
                return;
            }

            clip.displayName = displayName;
        }
    }
}