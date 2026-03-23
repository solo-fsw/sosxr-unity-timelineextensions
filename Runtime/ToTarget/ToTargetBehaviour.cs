using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour for the ToTarget track. Moves and rotates the bound GameObject smoothly toward a Target, calculating the required clip duration from the distance, ease curves, and configured speeds. Inherits from <see cref="PlayableBehaviour"/> directly (not the base <see cref="Behaviour"/>) because it handles its own frame processing logic.
    /// </summary>
    [Serializable]
    public class ToTargetBehaviour : PlayableBehaviour
    {
        public GameObject TrackBinding;
        public ToTargetClip ToTargetClip;

        [Space(20)]
        [Tooltip("Which axis to use for calculations? 0 = don't use, 1 = use")]
        public Vector3Int AxisToUse = new(1, 0, 1);

        public float RotateSpeed = 1.25f;
        public float MoveSpeed = 1.25f;
        public float StoppingDistance = 0.25f;
        public bool ForceClipLength = true;

        [Header("These are not for editing in the Inspector")]
        public float StartingDistance;

        public float StartMinStopDistance;
        public Vector2 DistanceWithEase;
        public float DistanceAtSpeed;
        public float DurationAtSpeed;
        public float DurationToTarget;
        public float remainingDistance;
        public float RemainingMinStopDistance;
        public Vector3 DisplacementFromTarget;
        public Vector3 DirectionToTarget;

        private Vector2 areaUnderCurves;
        private Vector2 _easeDuration;
        private Vector3 _velocity;

        public TimelineClip TimelineClip { get; set; }

        public GameObject StartingPoint { get; set; }

        public GameObject Target { get; set; }

        public override void OnGraphStart(Playable playable) => CalculateValues();

        private void CalculateValues()
        {
            if (StartingPoint == null || Target == null)
            {
                return;
            }

            DisplacementFromTarget = CalculateDisplacement(StartingPoint.transform, Target.transform);
            DirectionToTarget = CalculateDirection(DisplacementFromTarget);
            StartingDistance = CalculateDistance(DisplacementFromTarget);
            CalulateAreaUnderCurves(TimelineClip);
            SetEaseDuration(TimelineClip);
            CalculateRequiredDuration();
        }

        private void CalculateRequiredDuration()
        {
            DistanceWithEase.x = _easeDuration.x * areaUnderCurves.x * MoveSpeed;
            DistanceWithEase.y = _easeDuration.y * areaUnderCurves.y * MoveSpeed;
            StartMinStopDistance = StartingDistance - StoppingDistance;
            DistanceAtSpeed = StartMinStopDistance - (DistanceWithEase.x + DistanceWithEase.y);
            DurationAtSpeed = DistanceAtSpeed / MoveSpeed;
            DurationToTarget = _easeDuration.x + _easeDuration.y + DurationAtSpeed;
        }

        /// <summary>
        ///     How far & in what direction do I need to go?
        ///     For each axis in 'axisToUse' that is set to 0, the displacement will also be 0.
        /// </summary>
        /// <returns></returns>
        public Vector3 CalculateDisplacement(Transform originTrans, Transform targetTrans)
        {
            var displacement = targetTrans.position - originTrans.position;

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

            return displacement;
        }

        /// <summary>
        ///     Creates vector with max 1
        /// </summary>
        /// <param name="displacement"></param>
        /// <returns></returns>
        public static Vector3 CalculateDirection(Vector3 displacement) => displacement.normalized;

        /// <summary>
        ///     Calculetes how far away the target is.
        ///     Magnitude is the long side (C) of the triangle: A^2+B^2 = C^2
        /// </summary>
        /// <param name="displacement"></param>
        /// <returns></returns>
        public static float CalculateDistance(Vector3 displacement) => displacement.magnitude;

        /// <summary>
        ///     Here we set the clip duration to the length that's set by the values on the clip itself.
        /// </summary>
        /// <param name="clip"></param>
        private void SetClipDuration(TimelineClip clip)
        {
            if (Application.isPlaying) // Because otherwise any change to any clip during play results in the clipDuration to be reset to that specific duration
            {
                return;
            }

            if (clip == null)
            {
                return;
            }

            if (ForceClipLength)
            {
                clip.duration = DurationToTarget;
            }
        }

        private void SetEaseDuration(TimelineClip clip)
        {
            _easeDuration.x = (float)clip.easeInDuration;
            _easeDuration.y = (float)clip.easeOutDuration;
        }

        private void CalulateAreaUnderCurves(TimelineClip clip)
        {
            areaUnderCurves.x = CalculateAreaUnderCurve(clip.mixInCurve);
            areaUnderCurves.y = CalculateAreaUnderCurve(clip.mixOutCurve);
        }

        /// <summary>
        ///     From: https://blog.devgenius.io/calculating-the-area-under-an-animationcurve-in-unity-c43132a3abf8
        /// </summary>
        private static float CalculateAreaUnderCurve(AnimationCurve curve)
        {
            const float stepSize = 0.001f; // Small stepsize to increase precision

            float sum = 0;

            for (int i = 0; i < 1 / stepSize; i++)
            {
                sum += IntegralOnStep(stepSize * i, curve.Evaluate(stepSize * i), stepSize * (i + 1), curve.Evaluate(stepSize * (i + 1)));
            }

            return sum;
        }

        /// <summary>
        ///     From: https://blog.devgenius.io/calculating-the-area-under-an-animationcurve-in-unity-c43132a3abf8
        /// </summary>
        private static float IntegralOnStep(float x0, float y0, float x1, float y1)
        {
            float a = (y1 - y0) / (x1 - x0);
            float b = y0 - (a * x0);

            return (a / 2 * x1 * x1) + (b * x1) - ((a / 2 * x0 * x0) + (b * x0));
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            GameObject data = (GameObject)playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

            if (data == null)
            {
                return;
            }

            if (TrackBinding == null)
            {
                TrackBinding = data;
            }

            if (Target == null || StartingPoint == null)
            {
                return;
            }

            DisplacementFromTarget = CalculateDisplacement(TrackBinding.transform, Target.transform);
            DirectionToTarget = CalculateDirection(DisplacementFromTarget);
            remainingDistance = CalculateDistance(DisplacementFromTarget);
            RemainingMinStopDistance = remainingDistance - StoppingDistance;

#if UNITY_EDITOR
            DrawRay(TrackBinding.transform, DisplacementFromTarget);
#endif

            Move(info);
        }

        private void DrawRay(Transform originTrans, Vector3 displacement) => Debug.DrawRay(originTrans.position, displacement);

        private void Move(FrameData info)
        {
            if (Application.isPlaying)
            {
                HandleSmoothRotation(DirectionToTarget, info.deltaTime);
                HandleMovement(DirectionToTarget, info);
            }
        }

        /// <summary>
        ///     Rotates forward vector to target by speed
        /// </summary>
        /// <param name="direction"></param>
        private void HandleSmoothRotation(Vector3 direction, float deltaTime)
        {
            Vector3 newDirection = Vector3.RotateTowards(TrackBinding.transform.forward, direction, RotateSpeed * deltaTime, 0.0f);
            TrackBinding.transform.rotation = Quaternion.LookRotation(newDirection);
        }

        private void HandleMovement(Vector3 direction, FrameData info)
        {
            _velocity = direction * MoveSpeed * info.weight;

            if (remainingDistance >= StoppingDistance)
            {
                TrackBinding.transform.position += _velocity * info.deltaTime;
            }
        }
    }
}
