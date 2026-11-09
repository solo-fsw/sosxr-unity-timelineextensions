using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the RotateToTarget track. The track binding is the Transform that rotates. Each frame it slerps that
    ///     Transform from its start rotation toward a look-rotation at the clip's Rotator (the target), using the ease weight.
    ///     When the clip ends, the bound Transform gets its start rotation back.
    /// </summary>
    public class RotateToTargetMixer : Mixer
    {
        private const float MinSqrDistance = 1e-8f;

        private Transform _thingThatRotates;
        private Transform _target;
        private Quaternion _startRotation = Quaternion.identity;
        private RotateToTargetBehaviour _behaviour;

        protected override void InitializeMixer(Playable playable)
        {
            _thingThatRotates ??= TrackBinding as Transform;

            if (_thingThatRotates == null)
            {
                Debug.LogWarning($"{TypeName}: TrackBinding is not a Transform. Did you forget to set it?");
            }
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not RotateToTargetBehaviour behaviour)
            {
                return;
            }

            if (_thingThatRotates != null)
            {
                _startRotation = _thingThatRotates.rotation;
            }

            _behaviour = behaviour;
            _target = behaviour.Rotator;
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (_target == null || _behaviour == null || _thingThatRotates == null)
            {
                return;
            }

            if (activeBehaviour != _behaviour)
            {
                return;
            }

            var targetRotation = ComputeLookRotation(_thingThatRotates.position, _target.position, _behaviour.AxisToUse, _startRotation);

            _thingThatRotates.rotation = Quaternion.Slerp(_startRotation, targetRotation, easeWeight);

#if UNITY_EDITOR
            var easingOut = _behaviour.TimelineClip != null && _behaviour.EaseOutStarted; // EaseOutStarted logs an error without a TimelineClip
            Debug.DrawRay(_thingThatRotates.position, (easingOut ? _startRotation : targetRotation) * Vector3.forward, Color.magenta);
#endif
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour != _behaviour || _thingThatRotates == null)
            {
                return;
            }

            _thingThatRotates.rotation = _startRotation;
            _behaviour = null;
            _target = null;
        }

        /// <summary>
        ///     Look-rotation from <paramref name="from"/> toward <paramref name="to"/>, ignoring every axis whose
        ///     <paramref name="axisToUse"/> component is 0.
        /// </summary>
        /// <returns><paramref name="fallback"/> when the (masked) direction is zero, so LookRotation never gets a zero vector.</returns>
        public static Quaternion ComputeLookRotation(Vector3 from, Vector3 to, Vector3Int axisToUse, Quaternion fallback)
        {
            var displacement = to - from;

            if (axisToUse.x == 0)
            {
                displacement.x = 0;
            }

            if (axisToUse.y == 0)
            {
                displacement.y = 0;
            }

            if (axisToUse.z == 0)
            {
                displacement.z = 0;
            }

            if (displacement.sqrMagnitude < MinSqrDistance)
            {
                return fallback;
            }

            return Quaternion.LookRotation(displacement.normalized);
        }
    }
}
