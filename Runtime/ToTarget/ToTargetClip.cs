using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     This class us to set or see the values in the editor.
/// </summary>
[Serializable]
public class ToTargetClip : PlayableAsset
{
    public ExposedReference<GameObject> startingPoint;

    public ExposedReference<GameObject> target;

    public ToTargetBehaviour template = new();

    private TimelineClip timelineClip;

    private const string divider = " - ";
    public GameObject StartingPoint { get; set; }
    public GameObject Target { get; set; }

    public TimelineClip TimelineClip
    {
        get => timelineClip;
        set => timelineClip = value;
    }

    public ToTargetBehaviour Behaviour { get; set; }

    public override double duration
    {
        get
        {
            if (template.forceClipLength == false)
            {
                return base.duration;
            }

            if (Behaviour.durationToTarget == 0)
            {
                return base.duration;
            }

            return TimelineClip.duration = Behaviour.durationToTarget;
        }
    }


    /// <summary>
    ///     Here we write our logic for creating the playable behaviour
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ToTargetBehaviour>.Create(graph, template); // Create a playable using the constructor

        Behaviour = playable.GetBehaviour(); // Get behaviour

        if (StartingPoint == null)
        {
            StartingPoint = startingPoint.Resolve(graph.GetResolver());
        }

        if (Target == null)
        {
            Target = target.Resolve(graph.GetResolver());
        }

        SetValuesOnBehaviourFromClip(Behaviour);

        SetDisplayName(Behaviour, TimelineClip);

        return playable;
    }


    private void SetValuesOnBehaviourFromClip(ToTargetBehaviour behaviour)
    {
        behaviour.toTargetClip = this;
        behaviour.Target = Target;
        behaviour.StartingPoint = StartingPoint;
    }


    /// <summary>
    ///     The displayname of the clip in Timeline will be set using this method.
    ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
    /// </summary>
    private void SetDisplayName(ToTargetBehaviour behaviour, TimelineClip clip)
    {
        var displayName = "";

        if (behaviour.Target == null || behaviour.StartingPoint == null)
        {
            return;
        }

        displayName += "To: " + behaviour.Target.name;

        displayName += divider + behaviour.StartingPoint.name; // TODO: fix naming

        displayName = RemoveTrailingDivider(displayName);
        displayName = SetDisplayNameIfStillEmpty(displayName);

        if (clip == null)
        {
            return;
        }

        clip.displayName = displayName;
    }


    private static string RemoveTrailingDivider(string dispName)
    {
        if (string.IsNullOrEmpty(dispName))
        {
            return dispName;
        }

        var removeLast = dispName.LastIndexOf(divider, StringComparison.Ordinal);

        if (removeLast < 0)
        {
            return dispName;
        }

        dispName = dispName.Remove(removeLast);

        return dispName;
    }


    private static string SetDisplayNameIfStillEmpty(string dispName)
    {
        if (string.IsNullOrEmpty(dispName))
        {
            dispName = "New ToTarget Clip";
        }

        return dispName;
    }
}