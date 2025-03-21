using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class TimeControlClip : Clip
    {
        [HideInInspector] public TimeControlBehaviour Template;

        public TimeState InitialState; // This is what you set in the inspector for what this clip initially needs to do

        public override ClipCaps clipCaps => ClipCaps.None; // Do not allow blending between clips


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Template.InitialState = InitialState;
            Template.CurrentState = InitialState;

            var playable = ScriptPlayable<TimeControlBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip, TrackBinding);
            clone.SetDisplayName();

            return playable;
        }
    }
}