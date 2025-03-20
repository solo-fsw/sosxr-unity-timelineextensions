using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class AnimatorMixer : Mixer
    {
        public Animator Animator;


        protected override void ProcessingFrame()
        {
            if (Animator == null)
            {
                Animator = (Animator) TrackBinding;
            }
        }


        protected override void ActiveBehaviour(Behaviour genericActiveBehaviour, float easeWeight)
        {
            if (genericActiveBehaviour is not AnimatorBehaviour animatorBehaviour)
            {
                Debug.LogWarning("Couldn't cast to correct Behaviour implementation");

                return;
            }

            if (genericActiveBehaviour.ClipHasStartedOnce)
            {
                if (animatorBehaviour.StartClipStateName is "" or " " or "Default_State" or "None" or "NONE")
                {
                    return;
                }

                Animator.CrossFade(animatorBehaviour.StartClipStateName, genericActiveBehaviour.EaseInDuration, 0);
            }

            if (genericActiveBehaviour.EaseOutStartedOnce)
            {
                if (animatorBehaviour.EndClipStateName is "" or " " or "Default_State" or "None" or "NONE")
                {
                    return;
                }

                Animator.CrossFade(animatorBehaviour.EndClipStateName, genericActiveBehaviour.EaseOutDuration, 0);
            }
        }
    }
}