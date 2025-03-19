using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    public class Clip : TLClip
    {
        public ExposedReference<Transform> ExampleReference; // An exposed reference is on the Clip
        public readonly Behaviour Template = new();


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<Behaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();
            clone.TimelineClip = TimelineClip;

            clone.Example = ExampleReference.Resolve(graph.GetResolver());

            return playable;
        }
    }
}