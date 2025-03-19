using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     These variables allow us to set the value in the editor.
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [Serializable]
    public class AnimatorClip : PlayableAsset, ITimelineClipAsset
    {
        public AnimatorBehaviour Template;
        public TimelineClip TimelineClip { get; private set; }

        public ClipCaps clipCaps => ClipCaps.Blending;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AnimatorBehaviour>.Create(graph, Template);

            var clone = playable.GetBehaviour();
            
            clone.StartTransitionDuration = (float) TimelineClip.easeInDuration;
            clone.EndTransitionDuration = (float) TimelineClip.easeOutDuration;

            SetDisplayName(TimelineClip, Template);
            
            return playable;
        }


        /// <summary>
        ///     The displayName of the clip in Timeline will be set using this method.
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        public void SetDisplayName(TimelineClip clip, AnimatorBehaviour template)
        {
            var displayName = "";

            if (!string.IsNullOrEmpty(template.StartClipStateName))
            {
                displayName += "Clip Start State: " + template.StartClipStateName + " (" + template.StartTransitionDuration + "s)";
            }

            if (!string.IsNullOrEmpty(template.StartClipStateName) && !string.IsNullOrEmpty(template.EndClipStateName))
            {
                displayName += " || ";
            }

            if (!string.IsNullOrEmpty(template.EndClipStateName))
            {
                displayName += "On Clip End State: " + template.EndClipStateName + " (" + template.EndTransitionDuration + "s)";
            }

            displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Clip");

            if (clip == null)
            {
                return;
            }

            clip.displayName = displayName;
        }


        public void Initialize<T>(T trackBinding, TimelineClip timelineClip)
        {
            Template.TrackBinding = trackBinding as Animator;
            TimelineClip = timelineClip;
        }
    }
}