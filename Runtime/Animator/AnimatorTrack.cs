using UnityEngine;
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
        ///     This tells our track to use the trackMixer to control our playableBehaviours
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="go"></param>
        /// <param name="inputCount"></param>
        /// <returns></returns>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var trackBinding = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as Animator;

            if (trackBinding == null)
            {
                return Playable.Null;
            }

            foreach (var timelineClip in GetClips()) // Gets the TimelineClips from the Track
            {
                if (timelineClip.asset is not AnimatorClip clip)
                {
                    continue;
                }

                clip.Initialize(trackBinding, timelineClip);
            }

            return ScriptPlayable<AnimatorMixer>.Create(graph, inputCount);
        }
    }
}