using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Animator track. Drives state transitions via <c>Animator.CrossFadeInFixedTime</c>.
    ///     When two clips overlap, crossfades between states using each clip's ease-in duration as the blend time.
    ///     When the last clip eases out, crossfades back to the track's configured <c>DefaultState</c> (if one is set).
    ///
    ///     <para><b>Overlap Handling:</b> When Clip B starts during Clip A, <c>_activeBehaviour</c> becomes B.
    ///     When Clip A reaches ease-out, the <c>_activeBehaviour != behaviour</c> check causes early return,
    ///     preventing fade to default state. Only the last (non-overlapped) clip triggers the default state fade.</para>
    /// </summary>
    public class AnimatorMixer : Mixer
    {
        public Animator Binding;
        public AnimatorTrack Track;

        private AnimatorBehaviour _activeBehaviour;
        private string _currentState;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = TrackBinding as Animator;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(behaviour.StateName))
            {
                return;
            }

            _activeBehaviour = behaviour;
            CrossFadeToState(behaviour.StateName, behaviour.MixInDuration);
            _currentState = behaviour.StateName;
        }

        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            if (behaviour != _activeBehaviour) // A newer clip took over: it already crossfaded to its own state.
            {
                return;
            }

            _activeBehaviour = null;

            if (behaviour.AnotherClipOverlapsWithMe) // The next clip's ClipStarted handles the crossfade.
            {
                return;
            }

            if (Track == null || string.IsNullOrWhiteSpace(Track.DefaultState))
            {
                return;
            }

            if (_currentState == Track.DefaultState)
            {
                return;
            }

            CrossFadeToState(Track.DefaultState, behaviour.MixOutDuration);
            _currentState = Track.DefaultState;
        }

        protected virtual void CrossFadeToState(string stateName, float duration)
        {
            if (Binding == null || string.IsNullOrWhiteSpace(stateName))
            {
                return;
            }

            if (!Binding.HasState(stateName))
            {
                Debug.LogWarning($"{TypeName}: Animator '{Binding.name}' is missing state '{stateName}' in layer 0.");

                return;
            }

            Binding.CrossFadeInFixedTime(stateName, duration, 0);
        }
    }
}
