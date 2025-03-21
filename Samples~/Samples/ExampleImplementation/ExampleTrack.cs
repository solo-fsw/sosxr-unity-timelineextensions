using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    // [TrackColor(1, 0, .5f)]
    [TrackClipType(typeof(ExampleClip))]
    [TrackBindingType(typeof(Transform))] // Change binding here
    public class ExampleTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<ExampleMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();

            // Now you can do stuff with the Mixer
            mixer.ExampleMixerProperty = 42;

            return playable;
        }
    }
}