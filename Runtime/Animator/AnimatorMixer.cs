using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class AnimatorMixer : Mixer
    {
        public Animator Animator;


        public override void OnGraphStart(Playable playable)
        {
        }


        protected override void ActiveBehaviourClipStart(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                Debug.LogWarning("Couldn't cast to correct Behaviour implementation");

                return;
            }

            Animator ??= (Animator) TrackBinding;

            Animator.CrossFadeInFixedTime(behaviour.StartClipStateName, behaviour.EaseInDuration, 0);
        }


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                Debug.LogWarning("Couldn't cast to correct Behaviour implementation");

                return;
            }

            Animator ??= (Animator) TrackBinding;


            if (behaviour.EaseOutStartedOnce)
            {
                /*if (!Animator.HasState(behaviour.EndClipStateName))
                {
                    return;
                }*/

                Animator.CrossFadeInFixedTime(behaviour.EndClipStateName, behaviour.EaseOutDuration, 0);
            }
        }
    }
}