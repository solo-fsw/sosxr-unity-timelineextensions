using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    public class AnimatorMixer : Mixer<AnimatorBehaviour>
    {
        protected override void ActiveBehaviour<B>(B trackBinding, Behaviour genericActiveBehaviour, float easeWeight)
        {
            var animator = trackBinding as Animator;
            var animatorBehaviour = genericActiveBehaviour as AnimatorBehaviour;

            if (animatorBehaviour == null)
            {
                Debug.LogWarning("Couldn't cast to correct Behaviour implementation");

                return;
            }

            if (animator == null)
            {
                return;
            }

            if (genericActiveBehaviour.ClipHasStartedOnce)
            {
                animator.CrossFade(animatorBehaviour.StartClipStateName, genericActiveBehaviour.EaseInDuration, 0);
            }

            if (genericActiveBehaviour.EaseOutStartedOnce)
            {
                animator.CrossFade(animatorBehaviour.EndClipStateName, genericActiveBehaviour.EaseOutDuration, 0);
            }
        }
    }
}