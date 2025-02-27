using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     Based off of Unity Timeline sample pack : Time Dilation
///     From: https://docs.unity3d.com/Packages/com.unity.timeline@1.6/manual/smpl_about.html
/// </summary>
[Serializable]
public class TimeScaleClip : PlayableAsset, ITimelineClipAsset
{
    public TimeScaleBehaviour template = new();

    public TimelineClip TimelineClip { get; set; }

    public ClipCaps clipCaps => ClipCaps.Extrapolation | ClipCaps.Blending;


    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TimeScaleBehaviour>.Create(graph, template);
        var behaviour = playable.GetBehaviour();
        SetDisplayName(behaviour, TimelineClip);

        return playable;
    }


    /// <summary>
    ///     The displayname of the clip in Timeline will be set using this method.
    ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
    /// </summary>
    private void SetDisplayName(TimeScaleBehaviour behaviour, TimelineClip clip)
    {
        var displayName = "";

        displayName += "Time Scale: " + Math.Round(behaviour.timeScale, 3);

        displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Time Scale Clip");

        if (clip == null)
        {
            return;
        }

        clip.displayName = displayName;
    }
}