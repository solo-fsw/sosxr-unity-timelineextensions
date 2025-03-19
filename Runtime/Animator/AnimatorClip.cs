using UnityEngine;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     These variables allow us to set the value in the editor.
    /// </summary>
    public class AnimatorClip : Clip<AnimatorBehaviour>
    {
        public AnimatorBehaviour AnimatorTemplate;


        /// <summary>
        ///     The displayName of the clip in Timeline will be set using this method.
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        public static void SetDisplayName(TimelineClip clip, AnimatorBehaviour template)
        {
            var displayName = "";

            if (!string.IsNullOrEmpty(template.StartClipStateName))
            {
                displayName += "Clip Start State: " + template.StartClipStateName + " (" + template.EaseInDuration + "s)";
            }

            if (!string.IsNullOrEmpty(template.StartClipStateName) && !string.IsNullOrEmpty(template.EndClipStateName))
            {
                displayName += " || ";
            }

            if (!string.IsNullOrEmpty(template.EndClipStateName))
            {
                displayName += "On Clip End State: " + template.EndClipStateName + " (" + template.EaseOutDuration + "s)";
            }

            displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Clip");

            if (clip == null)
            {
                return;
            }

            clip.displayName = displayName;
        }


        public override void InitializeClip(IExposedPropertyTable resolver)
        {
            if (AnimatorTemplate != null)
            {
                return;
            }

            AnimatorTemplate = Template as AnimatorBehaviour;

            SetDisplayName(TimelineClip, AnimatorTemplate);
        }
    }
}