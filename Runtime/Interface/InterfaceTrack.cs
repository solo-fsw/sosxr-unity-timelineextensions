using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track for any component implementing <see cref="IInterface"/>.
    ///     No binding slot appears on the track header by design — use the "Pick IInterface Component…" button
    ///     in the Track Inspector instead. This is the single, filtered place to set and validate the binding.
    ///     Ideal for triggering arbitrary MonoBehaviour logic from Timeline without coupling the behaviour to it.
    /// </summary>
    [TrackClipType(typeof(InterfaceClip))]
    [NoTrackBinding]
    public class InterfaceTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            ScriptPlayable<InterfaceMixer> playable = ScriptPlayable<InterfaceMixer>.Create(
                graph,
                inputCount
            );

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
