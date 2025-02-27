using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackColor(0.855f, 0.903f, 0.87f)]
[TrackClipType(typeof(TimelineSpeedClip))]
public class TimelineSpeedTrack : TrackAsset
{
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

                var currentClip = (TimelineSpeedClip) clip.asset;
                currentClip.TimelineClip = clip;
                currentClip.template.TimelineClip = clip;
            }

            return handle;
        }

        return Playable.Null;
    }
}