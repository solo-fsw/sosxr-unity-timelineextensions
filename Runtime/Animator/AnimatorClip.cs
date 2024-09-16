using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     These variables allow us to set the value in the editor.
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable]
public abstract class AnimatorClip : PlayableAsset
{
    public AnimatorBehaviour template = new();

    private TimelineClip timelineClip;
    protected const string colon = ":";
    protected const string divider = " - ";

    public TimelineClip TimelineClip
    {
        get => timelineClip;
        set => timelineClip = value;
    }


    /// <summary>
    ///     Here we write our logic for creating the playable behaviour
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<AnimatorBehaviour>.Create(graph, template); // Create a playable using the constructor

        var behaviour = playable.GetBehaviour(); // Get behaviour

        SetClipOnBehaviour(behaviour);

        SetValuesOnBehaviourFromClip(behaviour);

        SetClipDuration(TimelineClip);

        SetOverallDisplayName(TimelineClip);

        return playable;
    }


    protected virtual void SetClipOnBehaviour(AnimatorBehaviour behaviour)
    {
        //	behaviour.animatorClip = this;
    }


    /// <summary>
    ///     Here we set the clip duration to the length that's set by the values on the clip itself.
    /// </summary>
    /// <param name="clip"></param>
    protected virtual void SetClipDuration(TimelineClip clip)
    {
        // Currently only used in the TriggerClip.
    }


    protected abstract void SetValuesOnBehaviourFromClip(AnimatorBehaviour behaviour);


    /// <summary>
    ///     The displayname of the clip in Timeline will be set using this method.
    ///     Name is only set if a varable is used (in case of X/Y/Z if they have a value != 0, in other cases if the string
    ///     name of the variable is not null).
    ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
    /// </summary>
    private void SetOverallDisplayName(TimelineClip clip)
    {
        var dispName = SetDisplayName();

        dispName = RemoveTrailingDivider(dispName);
        dispName = SetDisplayNameIfStillEmpty(dispName);

        if (clip != null)
        {
            clip.displayName = dispName;
        }
    }


    protected abstract string SetDisplayName();


    private static string RemoveTrailingDivider(string dispName)
    {
        if (!string.IsNullOrEmpty(dispName))
        {
            var removeLast = dispName.LastIndexOf(divider, StringComparison.Ordinal);

            if (removeLast < 0)
            {
                return dispName;
            }

            dispName = dispName.Remove(removeLast);

            return dispName;
        }

        return dispName;
    }


    private static string SetDisplayNameIfStillEmpty(string dispName)
    {
        if (string.IsNullOrEmpty(dispName))
        {
            dispName = "New Animator Clip";
        }

        return dispName;
    }
}