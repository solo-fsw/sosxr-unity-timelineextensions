using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class RotateToTargetMixer : Mixer
    {
        private Transform _transform;


        protected override void InitializeMixer(Playable playable)
        {
            _transform ??= TrackBinding as Transform;

            if (_transform == null)
            {
                Debug.LogWarning("RotateToTargetMixer: TrackBinding is not a Transform, did you forget to set it?");
            }
        }


        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            var behaviour = activeBehaviour as RotateToTargetBehaviour;

            var rotator = behaviour.Rotator;

            Vector3 displacement;

            if (!behaviour.EaseOutStarted)
            {
                displacement = _transform.position - rotator.position;
            }
            else // Reverse rotation
            {
                displacement = rotator.position - _transform.position;
            }

            if (behaviour.AxisToUse.x == 0)
            {
                displacement.x = 0;
            }

            if (behaviour.AxisToUse.y == 0)
            {
                displacement.y = 0;
            }

            if (behaviour.AxisToUse.z == 0)
            {
                displacement.z = 0;
            }

            var directionToTarget = displacement.normalized;
            var targetRotation = Quaternion.LookRotation(directionToTarget);

            rotator.rotation = Quaternion.Slerp(rotator.rotation, targetRotation, easeWeight * Time.deltaTime * behaviour.EaseSpeed);

            Debug.DrawRay(rotator.position, displacement, Color.magenta);
        }
    }
}