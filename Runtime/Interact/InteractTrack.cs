using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[TrackBindingType(typeof(GameObject))] // Bind to whatever you need to have in the Timeline
[TrackClipType(typeof(InteractClip))] // Tell the track that it can create clips from this binding
[Serializable]
public class InteractTrack : TrackAsset
{
    /// <summary>
    ///     Tell our track to use the trackMixer to control our playableBehaviours
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="go"></param>
    /// <param name="inputCount"></param>
    /// <returns></returns>
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        foreach (var clip in GetClips())
        {
            var currentClip = clip.asset as InteractClip;

            if (currentClip != null)
            {
                var gameObject = (GameObject) go.GetComponent<PlayableDirector>().GetGenericBinding(this);

                if (gameObject != null && gameObject.GetComponent<IInteract>() != null)
                {
                    currentClip.Template.interact = gameObject.GetComponent<IInteract>();
                }
            }
        }

        return ScriptPlayable<InteractTrackMixer>.Create(graph, inputCount);
    }
}