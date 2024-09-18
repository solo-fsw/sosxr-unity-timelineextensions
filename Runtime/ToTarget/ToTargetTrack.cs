using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     This creates the TrackMixer, and sets the name of the Clip.
///     It also sets the duration of the clip, if so desired.
/// </summary>
[TrackColor(0.319f, 0.177f, 0.109f)]
[TrackBindingType(typeof(GameObject))] // Bind to whatever you need to control in Timeline
[TrackClipType(typeof(ToTargetClip))] // Tell the track that it can create clips from said binding
public class ToTargetTrack : TrackAsset
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

                var currentClip = (ToTargetClip) clip.asset;
                currentClip.TimelineClip = clip;
                currentClip.template.TimelineClip = clip;
                currentClip.template.trackBinding = (GameObject) gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this); // provides the playable asset with reference to the gameobject binding on the track.
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
        return ScriptPlayable<ToTargetTrackMixer>.Create(graph, inputCount);
    }
}