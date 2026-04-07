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

        //private string _currentState;
        private AnimatorBehaviour _activeBehaviour;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = (Animator)TrackBinding;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (Track == null || string.IsNullOrWhiteSpace(Track.DefaultState))
            {
                return;
            }

            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(behaviour.StateName))
            {
                return;
            }

            CrossFadeToState(behaviour.StateName, behaviour.MixInDuration);
        }

        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
            if (Track == null || string.IsNullOrWhiteSpace(Track.DefaultState))
            {
                return;
            }

            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            if (behaviour.AnotherClipOverlapsWithMe == false) // When there is no clip overlapping with this one, crossfade to the Track's default state. If there is an overlap, we're relying on the ClipStarted method to crossfade to the new state.
            {
                CrossFadeToState(Track.DefaultState, behaviour.MixOutDuration);
            }
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
