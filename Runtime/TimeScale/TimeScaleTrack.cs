using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     Based off of Unity Timeline sample pack : Time Dilation
///     From: https://docs.unity3d.com/Packages/com.unity.timeline@1.6/manual/smpl_about.html
/// </summary>
[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(TimeScaleClip))]
public class TimeScaleTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<TimeScaleTrackMixer>.Create(graph, inputCount);
    }


    /// <summary>
    ///     Overwritten because this allows us to send the TimeLineClip over
    /// </summary>
    protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
    {
        if (!graph.IsValid())
        {
            throw new ArgumentException("graph must be a valid PlayableGraph");
        }

        if (clip == null)
        {
            throw new ArgumentNullException(nameof(clip));
        }

        if (clip.asset is IPlayableAsset asset)
        {
            var handle = asset.CreatePlayable(graph, gameObject);

            if (handle.IsValid())
            {
                handle.SetAnimatedProperties(clip.curves);
                handle.SetSpeed(clip.timeScale);

                var currentClip = (TimeScaleClip) clip.asset;
                currentClip.TimelineClip = clip;
            }

            return handle;
        }

        return Playable.Null;
    }
}