using System;
using UnityEngine;
using UnityEngine.Playables;


/// <summary>
///     Acts as our data for the clip to write to
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable]
public class RigidbodyBehaviour : PlayableBehaviour
{
    public Rigidbody trackBinding;
    public bool isKinematic;
    public bool useGravity;
    public bool addForce;
    public float amount;
    public Transform target;
    public ForceMode forceMode;
    private Vector3 direction;
    private Vector3 displacement;
    private bool behaviourCompleted;


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (trackBinding == null && playerData != null)
        {
            trackBinding = playerData as Rigidbody;
        }

        if (!Application.isPlaying)
        {
            return;
        }

        if (behaviourCompleted)
        {
            return;
        }

        trackBinding.isKinematic = isKinematic;
        trackBinding.useGravity = useGravity;

        if (addForce == false)
        {
            behaviourCompleted = true;

            return;
        }

        displacement = CalculateDisplacement(trackBinding.transform, target);
        direction = CalculateDirection(displacement);
        trackBinding.AddForce(direction * amount, forceMode);

        behaviourCompleted = true;
    }


    /// <summary>
    ///     How far & in what direction do I need to go?
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculateDisplacement(Transform originTrans, Transform targetTrans)
    {
        return displacement = targetTrans.position - originTrans.position;
    }


    /// <summary>
    ///     Creates Vector with max 1
    /// </summary>
    /// <param name="displacement"></param>
    /// <returns></returns>
    public static Vector3 CalculateDirection(Vector3 displacement)
    {
        return displacement.normalized;
    }


    private static void DrawRay(Transform originTrans, Vector3 displacement)
    {
        Debug.DrawRay(originTrans.position, displacement);
    }
}