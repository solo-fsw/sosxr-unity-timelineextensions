using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     This creates the TrackMixer, and sets the name of the Clip.
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[TrackBindingType(typeof(Light))] // Bind to whatever you need to control in Timeline
[TrackClipType(typeof(LightsClip))] // Tell the track that it can create clips from said binding
public class LightsTrack : TrackAsset
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

                var currentClip = (LightsClip) clip.asset;
                currentClip.TimelineClip = clip;
            }

            return handle;
        }

        return Playable.Null;
    }


	/// <summary>
	///     This tells our track to use the trackMixer to control our playableBehaviours
	/// </summary>
	/// <param name="graph"></param>
	/// <param name="go"></param>
	/// <param name="inputCount"></param>
	/// <returns></returns>
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<LightsTrackMixer>.Create(graph, inputCount);
    }
}