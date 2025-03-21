using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     These variables allow us to set the value in the editor.
    /// </summary>
    public class AnimatorClip : Clip
    {
        public AnimatorBehaviour Template;

        [HideInInspector] public List<string> StateNames;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var animator = TrackBinding as Animator;

            var playable = ScriptPlayable<AnimatorBehaviour>.Create(graph, Template);

            if (Template.EndClipStateName == "Default_State")
            {
                Template.EndClipStateName = animator.GetDefaultEntryStateName();
            }

            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            StateNames = new List<string>();
            StateNames = animator.GetStateNames();

            SetDisplayName(TimelineClip, Template);

            return playable;
        }


        /// <summary>
        ///     The displayName of the clip in Timeline will be set using this method.
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        private static void SetDisplayName(TimelineClip clip, AnimatorBehaviour template)
        {
            var displayName = "";

            if (!string.IsNullOrEmpty(template.StartClipStateName))
            {
                displayName += "ClipStart: " + template.StartClipStateName + " (" + template.EaseInDuration + "s)";
            }

            if (!string.IsNullOrEmpty(template.StartClipStateName) && !string.IsNullOrEmpty(template.EndClipStateName))
            {
                displayName += " || ";
            }

            if (!string.IsNullOrEmpty(template.EndClipStateName))
            {
                displayName += "ClipEnd: " + template.EndClipStateName + " (" + template.EaseOutDuration + "s)";
            }

            displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Clip");

            if (clip == null)
            {
                return;
            }

            clip.displayName = displayName;
        }


        public override void InitializeClip()
        {
        }
    }
}