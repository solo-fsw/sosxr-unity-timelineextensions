using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class RotateToTargetMixer : Mixer
    {
        private Transform _trackBinding;


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not RotateToTargetBehaviour behaviour)
            {
                return;
            }

            _trackBinding ??= TrackBinding as Transform;

            if (_trackBinding == null)
            {
                Debug.LogWarning("Couldn't cast to correct track binding");

                return;
            }

            var rotator = behaviour.Rotator;

            Vector3 displacement;

            if (!behaviour.EaseOutStarted)
            {
                displacement = _trackBinding.position - rotator.position;
            }
            else // Reverse rotation
            {
                displacement = rotator.position - _trackBinding.position;
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