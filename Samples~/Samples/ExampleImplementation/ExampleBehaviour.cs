using System;
using UnityEngine;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable] // Behaviours need to be serializable
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