using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Animator track. Drives state transitions via <c>Animator.CrossFadeInFixedTime</c>.
    ///     When two clips overlap, the actual overlap duration is calculated and used as the crossfade duration for a smooth
    ///     blend. When all clips end, crossfades back to the track's configured <c>DefaultState</c>.
    /// </summary>
    public class AnimatorMixer : Mixer
    {
        public Animator Binding;
        public AnimatorTrack Track;

        private string _currentState;
        private AnimatorBehaviour _activeBehaviour;
        private bool _hasActiveClip;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = (Animator)TrackBinding;
            _hasActiveClip = false;

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

            float blendDuration = behaviour.EaseInDuration;

            if (_hasActiveClip && _activeBehaviour != null)
            {
                float overlap = CalculateOverlap(behaviour);
                if (overlap > 0)
                {
                    blendDuration = overlap;
                }
            }

            CrossFadeToState(behaviour.StateName, blendDuration);
            _currentState = behaviour.StateName;
            _activeBehaviour = behaviour;
            _hasActiveClip = true;
        }

        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not AnimatorBehaviour behaviour)
            {
                return;
            }

            if (_activeBehaviour != behaviour)
            {
                return;
            }

            _hasActiveClip = false;
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

        private float CalculateOverlap(AnimatorBehaviour incomingBehaviour)
        {
            if (incomingBehaviour.TimelineClip == null || _activeBehaviour?.TimelineClip == null)
            {
                return 0f;
            }

            TimelineClip incoming = incomingBehaviour.TimelineClip;
            TimelineClip outgoing = _activeBehaviour.TimelineClip;

            double incomingStart = incoming.start;
            double outgoingEnd = outgoing.end;

            if (incomingStart < outgoingEnd)
            {
                return (float)(outgoingEnd - incomingStart);
            }

            return 0f;
        }

        private void CrossFadeToState(string stateName, float duration)
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
