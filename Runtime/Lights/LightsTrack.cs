using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.468f, 0.704f, 0.818f)]
    [TrackBindingType(typeof(Light))] // Bind to whatever you need to control in Timeline
    [TrackClipType(typeof(LightsClip))] // Tell the track that it can create clips from said binding
    public class LightsTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<LightsMixer>.Create(graph, inputCount);

            return playable;
        }
    }
}