using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [TrackClipType(typeof(InterfaceClip))] // Tell the track that it can create clips from this binding
    [Serializable]
    public class InterfaceTrack : TrackAsset
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

                    var currentClip = (InterfaceClip) clip.asset;
                    currentClip.TimelineClip = clip;
                }

                return handle;
            }

            return Playable.Null;
        }
    }
}