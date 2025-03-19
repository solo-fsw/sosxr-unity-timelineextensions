using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.855f, 0.8623f, 0.870f)]
    [TrackClipType(typeof(ExampleClip))]
    [TrackBindingType(typeof(Transform))] // Change binding here
    public class ExampleTrack : TLTrack
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