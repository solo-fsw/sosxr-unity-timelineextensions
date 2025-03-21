using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.818f, 0.581f, 0.468f)]
    [TrackBindingType(typeof(Rig))] // Bind to whatever I need to have in the Timeline
    [TrackClipType(typeof(RigClip))] // Tell the track that it can create clips from this binding
    public class RigTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<RigMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();

            return playable;
        }
    }
}