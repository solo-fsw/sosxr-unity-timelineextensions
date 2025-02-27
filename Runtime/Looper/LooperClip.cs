using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
public class LooperClip : PlayableAsset
{
    public LooperBehaviour behaviour;

    public TimelineClip TimelineClip { get; set; }


    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<LooperBehaviour>.Create(graph, behaviour);

        behaviour = playable.GetBehaviour(); // Set it directly to the behaviour
        behaviour.LooperClip = this;
        SetDisplayName(behaviour, TimelineClip);

        return playable;
    }


    /// <summary>
    ///     The displayName of the clip in Timeline will be set using this method.
    ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
    /// </summary>
    public void SetDisplayName(LooperBehaviour looperBehaviour, TimelineClip clip)
    {
        var displayName = "";

        if (looperBehaviour.RunningLooperState == LooperState.BreakLooping)
        {
            displayName = "● do not loop";
        }
        else if (looperBehaviour.RunningLooperState == LooperState.Looping)
        {
            displayName = "↩︎ loop clip";
        }
        else if (looperBehaviour.RunningLooperState == LooperState.GoToStart)
        {
            displayName = "← go to clip start";
        }
        else if (looperBehaviour.RunningLooperState == LooperState.GoToEnd)
        {
            displayName = "→ go to clip end";
        }

        if (looperBehaviour.LoopBreaker != null && looperBehaviour.LoopBreakerObject != null)
        {
            displayName += " || LoopBreaker: " + looperBehaviour.LoopBreakerObject.name;
        }

        displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Looper Clip");

        if (clip == null)
        {
            return;
        }

        clip.displayName = displayName;
    }
}