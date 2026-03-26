using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the RotateToTarget track. Each frame it slerps the clip's Rotator Transform toward the track-bound
    ///     Transform during ease-in, and away from it during ease-out, using the configured axes and ease speed.
    /// </summary>
    public class RotateToTargetMixer : Mixer
    {
        private Transform _thingThatRotates;
        private Transform _target;
        private Quaternion _startRotation = new Quaternion();
        private RotateToTargetBehaviour _behaviour;

        protected override void InitializeMixer(Playable playable)
        {
            _thingThatRotates ??= TrackBinding as Transform;

            if (_thingThatRotates == null)
            {
                Debug.LogWarning($"{GetType().Name}: TrackBinding is not a Transform. Did you forget to set it?");
            }
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (_thingThatRotates != null)
            {
                _startRotation = _thingThatRotates.rotation;
            }

            _behaviour = activeBehaviour as RotateToTargetBehaviour;

            if (_behaviour == null)
            {
                return;
            }

            _target = _behaviour.Rotator;
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (_target == null)
            {
                return;
            }
            if (_behaviour == null)
            {
                return;
            }

            var displacement = _target.position - _thingThatRotates.position;

            if (_behaviour.AxisToUse.x == 0)
            {
                displacement.x = 0;
            }

            if (_behaviour.AxisToUse.y == 0)
            {
                displacement.y = 0;
            }

            if (_behaviour.AxisToUse.z == 0)
            {
                displacement.z = 0;
            }

            var directionToTarget = displacement.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            _thingThatRotates.rotation = Quaternion.Slerp(
                _startRotation,
                targetRotation,
                easeWeight
            );

#if UNITY_EDITOR
            Debug.DrawRay(
                _thingThatRotates.position,
                (_behaviour.EaseOutStarted ? _startRotation : targetRotation) * Vector3.forward,
                Color.magenta
            );
#endif
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (_target == null)
            {
                return;
            }

            _target.rotation = _startRotation;
        }
    }
}
