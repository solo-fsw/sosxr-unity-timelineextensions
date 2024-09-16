using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     This acts as our data for the clip to write to
/// </summary>
[Serializable]
public class ToTargetBehaviour : PlayableBehaviour
{
    public GameObject trackBinding;
    public ToTargetClip toTargetClip;

    [Space(20)] [Tooltip("Which axis to use for calculations? 0 = don't use, 1 = use")]
    public Vector3Int axisToUse = new(1, 0, 1);

    public float rotateSpeed = 1.25f;
    public float moveSpeed = 1.25f;
    public float stoppingDistance = 0.25f;
    public bool forceClipLength = true;

    [Header("These are not for editing in the Inspector")]
    public float startingDistance;
    public float startMinStopDistance;
    public Vector2 distanceWithEase;
    public float distanceAtSpeed;
    public float durationAtSpeed;
    public float durationToTarget;
    public float remainingDistance;
    public float remainingMinStopDistance;
    public Vector3 displacementFromTarget;
    public Vector3 directionToTarget;

    private Vector2 areaUnderCurves;
    private Vector2 easeDuration;
    private Vector3 velocity;

    public TimelineClip TimelineClip { get; set; }

    public GameObject StartingPoint { get; set; }

    public GameObject Target { get; set; }


    public override void OnGraphStart(Playable playable)
    {
        CalculateValues();
    }


    private void CalculateValues()
    {
        if (StartingPoint == null || Target == null)
        {
            return;
        }

        displacementFromTarget = CalculateDisplacement(StartingPoint.transform, Target.transform);
        directionToTarget = CalculateDirection(displacementFromTarget);
        startingDistance = CalculateDistance(displacementFromTarget);
        CalulateAreaUnderCurves(TimelineClip);
        SetEaseDuration(TimelineClip);
        CalculateRequiredDuration();
    }


    private void CalculateRequiredDuration()
    {
        distanceWithEase.x = easeDuration.x * areaUnderCurves.x * moveSpeed;
        distanceWithEase.y = easeDuration.y * areaUnderCurves.y * moveSpeed;
        startMinStopDistance = startingDistance - stoppingDistance;
        distanceAtSpeed = startMinStopDistance - (distanceWithEase.x + distanceWithEase.y);
        durationAtSpeed = distanceAtSpeed / moveSpeed;
        durationToTarget = easeDuration.x + easeDuration.y + durationAtSpeed;
    }


    /// <summary>
    ///     How far & in what direction do I need to go?
    ///     For each axis in 'axisToUse' that is set to 0, the displacement will also be 0.
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculateDisplacement(Transform originTrans, Transform targetTrans)
    {
        var displacement = targetTrans.position - originTrans.position;

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

        return displacement;
    }


    /// <summary>
    ///     Creates vector with max 1
    /// </summary>
    /// <param name="displacement"></param>
    /// <returns></returns>
    public static Vector3 CalculateDirection(Vector3 displacement)
    {
        return displacement.normalized;
    }


    /// <summary>
    ///     Calculetes how far away the target is.
    ///     Magnitude is the long side (C) of the triangle: A^2+B^2 = C^2
    /// </summary>
    /// <param name="displacement"></param>
    /// <returns></returns>
    public static float CalculateDistance(Vector3 displacement)
    {
        return displacement.magnitude;
    }


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

        if (forceClipLength)
        {
            clip.duration = durationToTarget;
        }
    }


    private void SetEaseDuration(TimelineClip clip)
    {
        easeDuration.x = (float) clip.easeInDuration;
        easeDuration.y = (float) clip.easeOutDuration;
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

        for (var i = 0; i < 1 / stepSize; i++)
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
        var a = (y1 - y0) / (x1 - x0);
        var b = y0 - a * x0;

        return a / 2 * x1 * x1 + b * x1 - (a / 2 * x0 * x0 + b * x0);
    }


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var data = (GameObject) playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

        if (data == null)
        {
            return;
        }

        if (trackBinding == null)
        {
            trackBinding = data;
        }

        if (Target == null)
        {
            return;
        }

        displacementFromTarget = CalculateDisplacement(trackBinding.transform, Target.transform);
        directionToTarget = CalculateDirection(displacementFromTarget);
        remainingDistance = CalculateDistance(displacementFromTarget);
        remainingMinStopDistance = remainingDistance - stoppingDistance;

        DrawRay(trackBinding.transform, displacementFromTarget);

        Move(info);
    }


    private void DrawRay(Transform originTrans, Vector3 displacement)
    {
        Debug.DrawRay(originTrans.position, displacement);
    }


    private void Move(FrameData info)
    {
        if (Application.isPlaying)
        {
            HandleSmoothRotation(directionToTarget);
            HandleMovement(directionToTarget, info);
        }
    }


    /// <summary>
    ///     Rotates forward vector to target by speed
    /// </summary>
    /// <param name="direction"></param>
    private void HandleSmoothRotation(Vector3 direction)
    {
        var newDirection = Vector3.RotateTowards(trackBinding.transform.forward, direction, rotateSpeed * Time.deltaTime, 0.0f);
        trackBinding.transform.rotation = Quaternion.LookRotation(newDirection);
    }


    private void HandleMovement(Vector3 direction, FrameData info)
    {
        velocity = direction * moveSpeed * info.weight;

        if (remainingDistance >= stoppingDistance)
        {
            trackBinding.transform.position += velocity * Time.deltaTime;
        }
    }
}