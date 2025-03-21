using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This creates the TrackMixer, and sets the name of the Clip.
    /// </summary>
    [TrackColor(0.506f, 0.255f, 0f)]
    [TrackBindingType(typeof(Animator))] // Bind to whatever you need to control in Timeline
    [TrackClipType(typeof(AnimatorClip))] // Tell the track that it can create clips from said binding
    public class AnimatorTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<AnimatorMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();
            mixer.TrackBinding = TrackBinding;

            return playable;
        }
    }
}