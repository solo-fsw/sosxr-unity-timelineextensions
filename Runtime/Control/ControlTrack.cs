using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackClipType(typeof(ControlClip))] // Tell the track that it can create clips from this binding
    [TrackBindingType(typeof(GameObject))] // Change binding here
    public class ControlTrack : Track
    {
        public IControl InterfaceTrackBinding;


        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<ControlMixer>.Create(graph, inputCount);

            if (!playable.IsValid())
            {
                Debug.LogWarning("Mixer is not valid");

                return Playable.Null;
            }

            var mixer = playable.GetBehaviour();

            var gameObject = TrackBinding as GameObject;
            InterfaceTrackBinding = gameObject?.GetComponent<IControl>();

            if (InterfaceTrackBinding == null)
            {
                Debug.LogWarning("No IPlayableControl found on " + (gameObject?.name ?? "Unknown GameObject"));
            }

            mixer.SetInterfaceTrackBinding(InterfaceTrackBinding);

            return playable;
        }
    }
}