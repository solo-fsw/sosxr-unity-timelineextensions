using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackClipType(typeof(InterfaceClip))] // Tell the track that it can create clips from this binding
    [TrackBindingType(typeof(GameObject))] // Change binding here
    public class InterfaceTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<InterfaceMixer>.Create(graph, inputCount);

            if (!playable.IsValid())
            {
                Debug.LogWarning("Mixer is not valid");

                return Playable.Null;
            }

            var mixer = playable.GetBehaviour();
            mixer.TrackBinding = TrackBinding;

            return playable;
        }
    }
}