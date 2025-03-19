using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    public class ExampleClip : Clip
    {
        public ExposedReference<Transform> ExampleReference; // An exposed reference is on the Clip
        public readonly ExampleBehaviour Template = new();


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ExampleBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();
            clone.TimelineClip = TimelineClip;

            clone.Example = ExampleReference.Resolve(graph.GetResolver());

            return playable;
        }
    }
}