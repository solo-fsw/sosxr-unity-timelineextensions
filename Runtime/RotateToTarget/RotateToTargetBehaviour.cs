using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class RotateToTargetBehaviour : Behaviour
    {
        [Space(20)]
        [Tooltip("Which axis to use for calculations? 0 = don't use, 1 = use")]
        public Vector3Int AxisToUse = new(1, 0, 1);
        public float RotateSpeed = 1.25f;

        public Vector3 DisplacementFromTarget;
        public Vector3 DirectionToTarget;
        public float DistanceToTarget;

        private GameObject _trackBinding;

        public GameObject Target { get; set; }


        public override void OnGraphStart(Playable playable)
        {
            CalculateValues();
        }


        public void CalculateValues()
        {
            if (Target == null)
            {
                return;
            }

            _trackBinding = TrackBinding as GameObject;

            if (_trackBinding == null)
            {
                return;
            }

            var displacement = Target.transform.position - _trackBinding.transform.position;

            if (AxisToUse.x == 0)
            {
                displacement.x = 0;
            }

            if (AxisToUse.y == 0)
            {
                displacement.y = 0;
            }

            if (AxisToUse.z == 0)
            {
                displacement.z = 0;
            }

            DirectionToTarget = displacement.normalized;
            DistanceToTarget = displacement.magnitude;
        }


        public void DrawRayInRotationDirection()
        {
            if (_trackBinding == null)
            {
                return;
            }

            Debug.DrawRay(_trackBinding.transform.position, DisplacementFromTarget, Color.red);
        }


        /// <summary>
        ///     Rotates forward vector to target by speed
        /// </summary>
        /// <param name="direction"></param>
        public void HandleSmoothRotation(Vector3 direction)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_trackBinding == null)
            {
                return;
            }

            var newDirection = Vector3.RotateTowards(_trackBinding.transform.position, direction, RotateSpeed * Time.deltaTime, 0.0f);
            _trackBinding.transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}