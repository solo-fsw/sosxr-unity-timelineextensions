using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This acts as our data for the clip to write to
    /// </summary>
    [Serializable]
    public class AnimatorBehaviour : Behaviour
    {
        public string StartClipStateName = "";
        public string EndClipStateName = "Default_State";
        public List<string> StateNames;


        public override void Initialize()
        {
            if (StateNames == null || StateNames.Count == 0)
            {
                var anim = (Animator) TrackBinding;
                StateNames = UpdateStateList(anim);
                
                Debug.LogWarning("State names updated : " + StateNames.Count);
            }
        }


        private List<string> UpdateStateList(Animator anim)
        {
            var stateNames = new List<string>();
            stateNames.Add("NONE");

            if (anim == null)
            {
                Debug.LogWarning("Animator is null");

                return stateNames;
            }

            if (anim.runtimeAnimatorController is not AnimatorController controller)
            {
                Debug.LogWarning("Animator controller is null");

                return stateNames;
            }

            foreach (var layer in controller.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    stateNames.Add(state.state.name);
                }
            }

            return stateNames;
        }
    }
}