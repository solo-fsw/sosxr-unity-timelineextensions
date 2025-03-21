using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable] // Clips need to be serializable
    public class ExampleClip : Clip
    {
        public ExampleBehaviour Template;
        public ExposedReference<Transform> ExampleReference; // An exposed reference is on the Clip


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ExampleBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();

            clone.InitializeBehaviour(TimelineClip, TrackBinding); // This really should be called here, since it allows you to set up the Behaviour (the clone!) with the correct data

            clone.Example = ExampleReference.Resolve(Resolver); // Resolve the ExposedReference to the actual object

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