using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class AnimatorMixer : Mixer
    {
        public Animator Animator;


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                Debug.LogWarning("Couldn't cast to correct Behaviour implementation");

                return;
            }

            Animator ??= (Animator) TrackBinding;

            if (behaviour.ClipStartedOnce)
            {
                if (!Animator.HasState(behaviour.StartClipStateName))
                {
                    return;
                }

                Animator.CrossFadeInFixedTime(behaviour.StartClipStateName, behaviour.EaseInDuration, 0);
            }

            if (behaviour.EaseOutStartedOnce)
            {
                if (!Animator.HasState(behaviour.EndClipStateName))
                {
                    return;
                }

                Animator.CrossFadeInFixedTime(behaviour.EndClipStateName, behaviour.EaseOutDuration, 0);
            }
        }
    }
}