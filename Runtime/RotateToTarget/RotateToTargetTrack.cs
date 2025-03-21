using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.319f, 0.177f, 0.109f)]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(RotateToTargetClip))]
    public class RotateToTargetTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<RotateToTargetMixer>.Create(graph, inputCount);

            return playable;
        }
    }
}