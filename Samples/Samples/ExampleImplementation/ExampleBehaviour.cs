using System;
using UnityEngine;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Minimal example Behaviour demonstrating how to extend the base <see cref="Behaviour"/> class.
    ///     Shows the required <see cref="SerializableAttribute"/> and proper <see cref="InitializeBehaviour"/> override.
    /// </summary>
    [Serializable]
    public class ExampleBehaviour : Behaviour
    {
        public Transform Example; // Data is stored on the Behaviour


        public override void InitializeBehaviour(TimelineClip timelineClip, object trackBinding)
        {
            base.InitializeBehaviour(timelineClip, trackBinding); // Always call this first

            Debug.Log("Any other Behaviour initialization code goes here.");
        }
    }
}