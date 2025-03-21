using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.468f, 0.704f, 0.818f)]
    [TrackBindingType(typeof(Transform))] // Bind to whatever you need to have in the Timeline
    [TrackClipType(typeof(ParentingClip))] // Tell the track that it can create clips from this binding
    public class ParentingTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<ParentingMixer>.Create(graph, inputCount);

            return playable;
        }
    }
}