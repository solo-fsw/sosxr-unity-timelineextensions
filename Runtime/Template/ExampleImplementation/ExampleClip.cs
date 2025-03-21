using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


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
        ///     Make sure to call the base method when overriding this method.
        /// </summary>
        /// <param name="trackBinding"></param>
        /// <param name="timelineClip"></param>
        /// <param name="resolver"></param>
        public override void InitializeClip(object trackBinding, TimelineClip timelineClip, IExposedPropertyTable resolver)
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);

            // Do other stuff here
        }
    }
}