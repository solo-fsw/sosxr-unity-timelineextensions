using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class AnimatorMixer : Mixer<AnimatorBehaviour>
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
                Animator.CrossFade(animatorBehaviour.StartClipStateName, genericActiveBehaviour.EaseInDuration, 0);
            }

            if (genericActiveBehaviour.EaseOutStartedOnce)
            {
                Animator.CrossFade(animatorBehaviour.EndClipStateName, genericActiveBehaviour.EaseOutDuration, 0);
            }
        }
    }
}