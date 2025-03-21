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

            mixer.TrackBinding = TrackBinding; // Good practice to set the TrackBinding here (cheaper, no runtime overhead), but if you forget, it also gets set in the ProcessFrame method of the Mixer

            return playable;
        }
    }
}