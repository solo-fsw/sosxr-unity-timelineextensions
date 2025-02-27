using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
public class TimelineSpeedClip : PlayableAsset, ITimelineClipAsset
{
    public TimelineSpeedBehaviour template = new();
    private TimelineClip _timelineClip;

    public TimelineClip TimelineClip
    {
        get => _timelineClip;
        set => _timelineClip = value;
    }


    public ClipCaps clipCaps { get; }


    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TimelineSpeedBehaviour>.Create(graph, template);
        var behaviour = playable.GetBehaviour();
        SetDisplayName(behaviour, TimelineClip);

        return playable;
    }


    /// <summary>
    ///     The displayname of the clip in Timeline will be set using this method.
    ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
    /// </summary>
    private void SetDisplayName(TimelineSpeedBehaviour behaviour, TimelineClip clip)
    {
        var displayName = "";

        if (behaviour.setSpeedAtStart)
        {
            displayName += "Start speed: " + Math.Round(behaviour.speedAtStart, 3);
            displayName += CustomPlayableClipHelper.Divider;
        }

        if (behaviour.setSpeedAtEnd)
        {
            displayName += "End speed: " + Math.Round(behaviour.speedAtEnd, 3);
            displayName += CustomPlayableClipHelper.Divider;
        }

        if (behaviour.setSpeedAtStart == false && behaviour.setSpeedAtEnd == false)
        {
            displayName = "";
        }

        displayName = CustomPlayableClipHelper.RemoveTrailingDivider(displayName);
        displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Speed Clip");

        if (clip == null)
        {
            return;
        }

        clip.displayName = displayName;
    }
}