using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    public class Clip : PlayableAsset, ITimelineClipAsset
    {
        public Behaviour Template = new();
        public ExposedReference<Transform> ExampleReference; // An exposed reference is on the Clip
        public TimelineClip TimelineClip { get; private set; }
        public Transform TrackBinding { get; private set; }

        public ClipCaps clipCaps { get; }


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<Behaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();
            clone.Example = ExampleReference.Resolve(graph.GetResolver());

            return playable;
        }
        
        public void Initialize<T>(T trackBinding, TimelineClip timelineClip)
        {
            TrackBinding = trackBinding as Transform;
            TimelineClip = timelineClip;
        }
    }
}