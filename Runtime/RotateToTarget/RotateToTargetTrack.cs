using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track that binds to a <see cref="Transform"/> (the object that rotates) and creates <see cref="RotateToTargetClip"/> clips.
    ///     The bound Transform smoothly turns to look at the clip's Rotator (the look-at target) while the clip is active.
    /// </summary>
    [TrackColor(0.319f, 0.177f, 0.109f)]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(RotateToTargetClip))]
    public class RotateToTargetTrack : Track
    {
        /// <summary>Creates a <see cref="RotateToTargetMixer"/> and passes it the track binding.</summary>
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<RotateToTargetMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();
            mixer.TrackBinding = TrackBinding;

            return playable;
        }
    }
}
