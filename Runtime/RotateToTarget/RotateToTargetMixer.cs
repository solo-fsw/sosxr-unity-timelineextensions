using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the RotateToTarget track. The track binding is the Transform that rotates. Each frame it slerps that
    ///     Transform from its start rotation toward a look-rotation at the clip's Rotator (the target), using the ease weight.
    ///     Overlapping clips: the newest clip drives, blending from the rotation it started at. When the last active clip
    ///     ends, the bound Transform gets the rotation it had before the first clip started back.
    /// </summary>
    public class RotateToTargetMixer : Mixer
    {
        private const float MinSqrDistance = 1e-8f;

        private Transform _thingThatRotates;
        private Transform _target;
        private Quaternion _startRotation = Quaternion.identity;

        /// <summary>Rotation before the first of a run of (overlapping) clips started. Restored when the last one ends.</summary>
        private Quaternion _restoreRotation = Quaternion.identity;
        private int _activeClipCount;
        private RotateToTargetBehaviour _behaviour;

        /// <summary>Caches the track binding as the Transform that rotates. Logs a warning when it is not a Transform.</summary>
        protected override void InitializeMixer(Playable playable)
        {
            _thingThatRotates ??= TrackBinding as Transform;

            if (_thingThatRotates == null)
            {
                Debug.LogWarning($"{TypeName}: TrackBinding is not a Transform. Did you forget to set it?");
            }
        }

        /// <summary>
        ///     Records the rotation to restore when the first of an overlapping run starts, takes the current rotation as the
        ///     slerp start, and makes this clip the driving one.
        /// </summary>
        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not RotateToTargetBehaviour behaviour)
            {
                return;
            }

            if (_thingThatRotates != null)
            {
                if (_activeClipCount == 0)
                {
                    _restoreRotation = _thingThatRotates.rotation;
                }

                _startRotation = _thingThatRotates.rotation;
            }

            _activeClipCount++;

            _behaviour = behaviour;
            _target = behaviour.Rotator;
        }

        /// <summary>
        ///     Slerps the bound Transform from its start rotation toward the look rotation at the clip's target, weighted by the
        ///     ease weight. Ignores clips that are not the driving clip.
        /// </summary>
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

        /// <summary>
        ///     Stops driving for the ended clip, hands control to another active clip when one exists, and restores the original
        ///     rotation once no clip is active.
        /// </summary>
        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not RotateToTargetBehaviour)
            {
                return;
            }

            if (_activeClipCount > 0)
            {
                _activeClipCount--;
            }

            if (activeBehaviour != _behaviour) // An older, overlapped clip ended: the newer clip keeps driving.
            {
                return;
            }

            _behaviour = null;
            _target = null;

            // The driving clip ended while an older clip is still active (nested clips): hand control back to it.
            foreach (var other in Behaviours)
            {
                if (other is RotateToTargetBehaviour { ClipIsActive: true } rotateBehaviour && other != activeBehaviour)
                {
                    _behaviour = rotateBehaviour;
                    _target = rotateBehaviour.Rotator;

                    return;
                }
            }

            if (_thingThatRotates == null || _activeClipCount > 0)
            {
                return;
            }

            _thingThatRotates.rotation = _restoreRotation;
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
