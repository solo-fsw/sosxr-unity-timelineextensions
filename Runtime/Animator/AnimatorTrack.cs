using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This creates the TrackMixer, and sets the name of the Clip.
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [TrackColor(0.745f, 0.414f, 0.255f)]
    [TrackBindingType(typeof(Animator))] // Bind to whatever you need to control in Timeline
    [TrackClipType(typeof(AnimatorClip))] // Tell the track that it can create clips from said binding
    public class AnimatorTrack : TrackAsset
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

            if (clip.asset is not IPlayableAsset asset)
            {
                return Playable.Null;
            }

            var playable = asset.CreatePlayable(graph, gameObject);

            if (!playable.IsValid())
            {
                return playable;
            }

            playable.SetAnimatedProperties(clip.curves);
            playable.SetSpeed(clip.timeScale);

            var currentClip = (AnimatorClip) clip.asset;
            currentClip.TimelineClip = clip;

            currentClip.Template.StartTransitionDuration = (float) clip.easeInDuration;
            currentClip.Template.EndTransitionDuration = (float) clip.easeOutDuration;

            var director = gameObject.GetComponent<PlayableDirector>();
            var animator = director.GetGenericBinding(this) as Animator;

            if (animator == null)
            {
                Debug.LogWarning("No binding found on the track");

                return playable;
            }


            if (currentClip.Template == null)
            {
                Debug.LogWarning("No template found on the clip");

                return playable;
            }

            currentClip.Template.TrackBinding = animator; // provides the playable asset with reference to the binding on the track.

            return playable;
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
            return ScriptPlayable<AnimatorTrackMixer>.Create(graph, inputCount);
        }
    }
}