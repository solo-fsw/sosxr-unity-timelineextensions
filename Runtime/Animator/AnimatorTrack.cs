using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track that binds to an <see cref="Animator"/> and creates <see cref="AnimatorClip"/> clips.
    ///     Use it to drive Animator state transitions directly from Timeline using CrossFade, with ease-in/out blending.
    /// </summary>
    [TrackColor(0.506f, 0.255f, 0f)]
    [TrackBindingType(typeof(Animator))] // Bind to whatever you need to control in Timeline
    [TrackClipType(typeof(AnimatorClip))] // Tell the track that it can create clips from said binding
    public class AnimatorTrack : Track
    {
        [Tooltip("The default/idle state to return to when no clips are active")]
        public string DefaultState = "Idle";

        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            ScriptPlayable<AnimatorMixer> playable = ScriptPlayable<AnimatorMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();

            if (mixer != null)
            {
                mixer.TrackBinding = TrackBinding;
                mixer.Track = this;
            }
            return playable;
        }
    }
}
