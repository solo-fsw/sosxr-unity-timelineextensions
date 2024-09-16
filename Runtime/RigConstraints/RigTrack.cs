using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[TrackColor(0.818f, 0.581f, 0.468f)]
[TrackBindingType(typeof(Rig))] // Bind to whatever I need to have in the Timeline
[TrackClipType(typeof(RigClip))] // Tell the track that it can create clips from this binding
[Serializable]
public class RigTrack : TrackAsset
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

                var currentClip = (RigClip) clip.asset;
                currentClip.TimelineClip = clip;
                currentClip.Template.trackBinding = (Rig) gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this); // provides the playable asset with reference to the Rig binding on the track.
            }

            return handle;
        }

        return Playable.Null;
    }


    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) // Tell our track to use the trackMixer to control our playableBehaviours
    {
        return ScriptPlayable<RigTrackMixer>.Create(graph, inputCount);
    }
}