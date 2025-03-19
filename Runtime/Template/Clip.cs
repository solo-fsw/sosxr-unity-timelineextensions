using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    public abstract class Clip<T> : PlayableAsset, ITimelineClipAsset, IClip where T : Behaviour, new()
    {
        protected TimelineClip TimelineClip { get; private set; }
        public Behaviour Template { get; private set; }


        public void InitializeClip(TimelineClip timelineClip)
        {
            TimelineClip = timelineClip;
        }


        public ClipCaps clipCaps => ClipCaps.Blending;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph, (T) Template);
            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip);

            return playable;
        }
    }
    
    public interface IClip
    {
        void InitializeClip(TimelineClip timelineClip);
    }
}