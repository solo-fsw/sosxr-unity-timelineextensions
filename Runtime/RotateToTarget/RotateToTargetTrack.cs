using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This creates the TrackMixer, and sets the name of the Clip.
    ///     It also sets the duration of the clip, if so desired.
    /// </summary>
    [TrackColor(0.319f, 0.177f, 0.109f)]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(RotateToTargetClip))]
// Bind to whatever you need to control in Timeline
// Tell the track that it can create clips from said binding
    public class RotateToTargetTrack : TrackAsset
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

            var handle = asset.CreatePlayable(graph, gameObject);

            if (!handle.IsValid())
            {
                return handle;
            }

            handle.SetAnimatedProperties(clip.curves);
            handle.SetSpeed(clip.timeScale);

            var currentClip = (RotateToTargetClip) clip.asset;
            currentClip.Template.TrackBinding = (GameObject) gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this);


            return handle;
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
            return ScriptPlayable<RotateToTargetTrackMixer>.Create(graph, inputCount);
        }
    }
}