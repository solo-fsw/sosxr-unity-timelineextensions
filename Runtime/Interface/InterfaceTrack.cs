using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track that binds directly to a <see cref="Component"/> that implements <see cref="IInterface"/>.
    ///     Drag the specific component (not the GameObject) into the binding slot — Unity will show a component picker
    ///     if you drop a GameObject. The track editor will flag an error if the bound component does not implement
    ///     <see cref="IInterface"/>.
    ///     Ideal for triggering arbitrary MonoBehaviour logic from Timeline without coupling the behaviour to it.
    /// </summary>
    [TrackClipType(typeof(InterfaceClip))]
    [TrackBindingType(typeof(Component))]
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
