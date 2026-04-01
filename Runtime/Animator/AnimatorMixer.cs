using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Animator track. Drives state transitions via <c>Animator.CrossFadeInFixedTime</c>.
    ///     When two clips overlap, crossfades between states using each clip's ease-in duration as the blend time.
    ///     When all clips end, crossfades back to the track's configured <c>DefaultState</c>.
    ///
    ///     <para><b>Overlap Handling:</b> When Clip B starts during Clip A, <c>_activeBehaviour</c> becomes B.
    ///     When Clip A reaches ease-out, the <c>_activeBehaviour != behaviour</c> check causes early return,
    ///     preventing fade to default state. Only the last (non-overlapped) clip triggers the default state fade.</para>
    /// </summary>
    public class AnimatorMixer : Mixer
    {
        public Animator Binding;
        public AnimatorTrack Track;

        private string _currentState;
        private AnimatorBehaviour _activeBehaviour;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = (Animator)TrackBinding;

            if (Track != null && !string.IsNullOrWhiteSpace(Track.DefaultState))
            {
                _currentState = Track.DefaultState;
            }
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

            // Crossfade to the new state using the clip's ease-in duration as blend time.
            // When clips overlap, this creates a smooth transition between states.
            CrossFadeToState(behaviour.StateName, behaviour.EaseInDuration);
            _currentState = behaviour.StateName;
            _activeBehaviour = behaviour;
        }

        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            // The _activeBehaviour check filters out overlapped clips:
            // When Clip B starts during Clip A, _activeBehaviour becomes B.
            // When Clip A reaches ease-out, _activeBehaviour != behaviour (A), so we return early.
            // This prevents fading to default state during overlap - the desired behavior.
            if (_activeBehaviour != behaviour)
            {
                return;
            }

            // At this point, this is the currently active clip (not overlapped by a newer one).
            // Clear state and fade to default if configured.
            _activeBehaviour = null;

            if (Track == null || string.IsNullOrWhiteSpace(Track.DefaultState))
            {
                return;
            }

            if (_currentState == Track.DefaultState)
            {
                return;
            }

            CrossFadeToState(Track.DefaultState, behaviour.EaseOutDuration);
            _currentState = Track.DefaultState;
        }

        protected virtual void CrossFadeToState(string stateName, float duration)
        {
            if (Binding == null || string.IsNullOrWhiteSpace(stateName))
            {
                return;
            }

#if UNITY_EDITOR
            if (!Binding.HasState(stateName))
            {
                Debug.LogWarning($"Animator is missing state '{stateName}' in controller layer 0.");
                return;
            }
#endif
            Binding.CrossFadeInFixedTime(stateName, duration, 0);
        }
    }
}
