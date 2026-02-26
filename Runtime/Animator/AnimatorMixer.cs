using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Animator track. Calls <c>Animator.CrossFadeInFixedTime</c> to the start state when a clip begins,
    ///     and to the end state once ease-out starts.
    /// </summary>
    public class AnimatorMixer : Mixer
    {
        public Animator Animator;


        protected override void InitializeMixer(Playable playable)
        {
            Animator ??= (Animator) TrackBinding;
        }


        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            var behaviour = activeBehaviour as AnimatorBehaviour;

            Animator.CrossFadeInFixedTime(behaviour.StartClipStateName, behaviour.EaseInDuration, 0);
        }


        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
            var behaviour = activeBehaviour as AnimatorBehaviour;

            Animator.CrossFadeInFixedTime(behaviour.EndClipStateName, behaviour.EaseOutDuration, 0);
        }
    }
}