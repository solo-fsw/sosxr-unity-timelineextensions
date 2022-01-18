using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackColor(0.7366781f, 0.3261246f, 0.8529412f)]
[TrackClipType(typeof (LooperClip))]
public class LooperTrack : TrackAsset
{
	/// <summary>
	/// Overwritten because this allows us to send the TimeLineClip over
	/// </summary>
	protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
	{
		if (!graph.IsValid())
			throw new ArgumentException("graph must be a valid PlayableGraph");
		if (clip == null)
			throw new ArgumentNullException(nameof (clip));

		if (clip.asset is IPlayableAsset asset)
		{
			var handle = asset.CreatePlayable(graph, gameObject);
			if (handle.IsValid())
			{
				handle.SetAnimatedProperties(clip.curves);
				handle.SetSpeed(clip.timeScale);

				var currentClip = (LooperClip) clip.asset;
				currentClip.behaviour.TimelineClip = clip;
				currentClip.TimelineClip = clip;
			}
			return handle;
		}
		return Playable.Null;
	}
}
