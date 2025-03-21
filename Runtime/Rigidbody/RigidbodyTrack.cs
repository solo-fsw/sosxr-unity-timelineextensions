using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.255f, 0.586f, 0.745f)]
    [TrackBindingType(typeof(Rigidbody))] // Bind to whatever you need to have in the Timeline
    [TrackClipType(typeof(RigidbodyClip))] // Tell the track that it can create clips from this binding
    public class RigidbodyTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<RigidbodyMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();

            return playable;
        }
    }
}