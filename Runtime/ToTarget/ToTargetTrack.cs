using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track that binds to a <see cref="GameObject"/> and creates <see cref="ToTargetClip"/> clips.
    ///     Moves and rotates the bound object toward a series of targets using ease-weighted, frame-rate-independent movement.
    /// </summary>
    [TrackColor(0.319f, 0.177f, 0.109f)]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(ToTargetClip))]
    public class ToTargetTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            return ScriptPlayable<ToTargetMixer>.Create(graph, inputCount);
        }
    }
}
