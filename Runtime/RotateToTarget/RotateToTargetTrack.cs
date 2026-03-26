using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track that binds to a <see cref="Transform"/> (the look-at target) and creates <see cref="RotateToTargetClip"/> clips.
    ///     The Rotator object in the clip smoothly looks toward the track binding while the clip is active.
    /// </summary>
    [TrackColor(0.319f, 0.177f, 0.109f)]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(RotateToTargetClip))]
    public class RotateToTargetTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<RotateToTargetMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();
            mixer.TrackBinding = TrackBinding;

            return playable;
        }
    }
}
