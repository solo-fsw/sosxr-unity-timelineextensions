using System.Collections.Generic;
using UnityEditor.Animations;
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
            var controller = animator?.runtimeAnimatorController as AnimatorController;

            var playable = ScriptPlayable<AnimatorBehaviour>.Create(graph, Template);

            if (Template.EndClipStateName == "Default_State")
            {
                Template.EndClipStateName = GetDefaultEntryStateName(controller);
            }

            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            UpdateStateList(animator);
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


        private void UpdateStateList(Animator anim)
        {
            StateNames = new List<string>();
            StateNames.Add("NONE");

            if (anim == null)
            {
                Debug.LogWarning("Animator is null");

                return;
            }

            if (anim.runtimeAnimatorController is not AnimatorController controller)
            {
                Debug.LogWarning("Animator controller is null");

                return;
            }

            foreach (var layer in controller.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    StateNames.Add(state.state.name);
                }
            }
        }


        private static string GetDefaultEntryStateName(AnimatorController controller)
        {
            if (controller == null)
            {
                Debug.LogWarning("Invalid animator controller");

                return "";
            }

            var stateMachine = controller.layers[0].stateMachine;

            return stateMachine.defaultState.name;
        }
    }
}