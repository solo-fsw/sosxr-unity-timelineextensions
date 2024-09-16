using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[TrackColor(0.468f, 0.704f, 0.818f)]
[TrackBindingType(typeof(GameObject))] // Bind to whatever you need to have in the Timeline
[TrackClipType(typeof(ParentingClip))] // Tell the track that it can create clips from this binding
[Serializable]
public class ParentingTrack : TrackAsset
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

                var currentClip = (ParentingClip) clip.asset;
                currentClip.TimelineClip = clip;
                currentClip.Template.trackBinding = (GameObject) gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this);
            }

            return handle;
        }

        return Playable.Null;
    }


	/// <summary>
	///     Tell our track to use the trackMixer to control our playableBehaviours
	/// </summary>
	/// <param name="graph"></param>
	/// <param name="go"></param>
	/// <param name="inputCount"></param>
	/// <returns></returns>
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<ParentingTrackMixer>.Create(graph, inputCount);
    }
}