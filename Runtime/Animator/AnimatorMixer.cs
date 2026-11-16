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
        /// <summary>The Animator the track is bound to. Set in <c>InitializeMixer</c>.</summary>
        public Animator Binding;
        /// <summary>The track asset whose <c>DefaultState</c> is faded to when the last clip eases out.</summary>
        public AnimatorTrack Track;

        private AnimatorBehaviour _activeBehaviour;
        private string _currentState;

        /// <summary>Caches the track binding as an <see cref="Animator"/>.</summary>
        protected override void InitializeMixer(Playable playable)
        {
            Binding = TrackBinding as Animator;
        }

        /// <summary>Crossfades to the started clip's state over its mix-in duration. Ignores clips without a state name.</summary>
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

        /// <summary>Takes control again after a seek back into a clip whose ease-out had started, and snaps the Animator to that clip's state.</summary>
        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            // A seek back into a clip whose ease-out had already started leaves nothing driving the Animator (ease-out
            // released it, and a seek inside a clip fires no new ClipStarted). Take the clip back, so its ease-out,
            // which fires again when reached, can fade to the default state.
            if (_activeBehaviour != null || activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            if (behaviour.TimelineClip == null || behaviour.EaseOutStarted || string.IsNullOrWhiteSpace(behaviour.StateName))
            {
                return;
            }

            _activeBehaviour = behaviour;

            if (_currentState == behaviour.StateName)
            {
                return;
            }

            CrossFadeToState(behaviour.StateName, 0f); // A seek is a jump: snap to the state.
            _currentState = behaviour.StateName;
        }

        /// <summary>
        ///     Fades back to the track's <c>DefaultState</c> when the last clip eases out. Does nothing when another clip overlaps
        ///     or no default state is set.
        /// </summary>
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

        /// <summary>
        ///     Crossfades the Animator on layer 0 to <paramref name="stateName"/>. Logs a warning and does nothing when the
        ///     state is missing or no Animator is bound.
        /// </summary>
        /// <param name="stateName">Name of the Animator state to fade to.</param>
        /// <param name="duration">Blend time in seconds.</param>
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
