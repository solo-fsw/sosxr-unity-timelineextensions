using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(1, 0, .5f)]
    [TrackClipType(typeof(ExampleClip))]
    [TrackBindingType(typeof(Transform))] // Change binding here
    public class ExampleTrack : Track
    {
        protected override Type GetBindingType()
        {
            return typeof(Transform);
        }


        protected override Playable CreateMixerPlayable(PlayableGraph graph, int inputCount)
        {
            return ScriptPlayable<ExampleMixer>.Create(graph, inputCount);
        }
    }
}