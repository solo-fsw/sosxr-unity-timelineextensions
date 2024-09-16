using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;


/// <summary>
///     Act as our data for the clip to write to
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable]
public class RigBehaviour : PlayableBehaviour
{
    public RigClip rigClip;
    public bool reset;
    public float resetToValue;
    public Transform rigTarget;
    public Transform worldTarget;
    public AnimationCurve rigWeight;
    public Rig trackBinding;
    private float time = 0;


    public override void ProcessFrame(Playable playable, FrameData info, object playerData) // Tell playable what to do when the playhead is on this clip
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var data = (Rig) playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

        if (!data)
        {
            return;
        }

        if (trackBinding == null)
        {
            trackBinding = data;
        }

        if (worldTarget == null)
        {
            return;
        }

        rigTarget.position = worldTarget.position;
        rigTarget.rotation = worldTarget.rotation;
        time += Time.deltaTime;
        trackBinding.weight = rigWeight.Evaluate((float) (time / rigClip.TimelineClip.duration)) * info.weight; // to set the value
    }


    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (trackBinding == null)
        {
            return;
        }

        if (rigClip == null)
        {
            return;
        }

        if (rigClip.reset)
        {
            trackBinding.weight = rigClip.resetToValue;
        }
    }
}