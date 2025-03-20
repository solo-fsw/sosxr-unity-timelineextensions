using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class ExampleClip : Clip
    {
        public ExampleBehaviour Template;
        public ExposedReference<Transform> ExampleReference; // An exposed reference is on the Clip


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ExampleBehaviour>.Create(graph, Template);

            var clone = playable.GetBehaviour();
            clone.TimelineClip = TimelineClip;
            clone.TrackBinding = TrackBinding;
            clone.InitializeBehaviour();

            Template.Example = ExampleReference.Resolve(Resolver);

            return playable;
        }


        /// <summary>
        ///     It's good practice to use this for anything in the Clip that needs setting up.
        ///     It gets called when the Clip is created from the Track.
        /// </summary>
        public override void InitializeClip()
        {
        }
    }
}