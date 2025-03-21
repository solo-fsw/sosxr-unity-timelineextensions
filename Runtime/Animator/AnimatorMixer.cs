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
            if (genericActiveBehaviour is not AnimatorBehaviour behaviour)
            {
                Debug.LogWarning("Couldn't cast to correct Behaviour implementation");

                return;
            }

            if (behaviour.ClipHasStartedOnce)
            {
                if (!Animator.CanTransitionTo(behaviour.StartClipStateName))
                {
                    return;
                }

                Animator.CrossFade(behaviour.StartClipStateName, behaviour.EaseInDuration, 0);
            }

            if (behaviour.EaseOutStartedOnce)
            {
                if (!Animator.CanTransitionTo(behaviour.EndClipStateName))
                {
                    return;
                }

                Animator.CrossFade(behaviour.EndClipStateName, genericActiveBehaviour.EaseOutDuration, 0);
            }
        }
    }
}