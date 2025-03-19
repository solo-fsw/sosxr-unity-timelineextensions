using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public abstract class Clip<T> : PlayableAsset, ITimelineClipAsset, IClip where T : Behaviour, new()
    {
        public Behaviour Template { get; private set; }
        public Behaviour Clone { get; private set; } // This is already the specific instance of the (derived) Behaviour
        public TimelineClip TimelineClip { get; set; }
        public IExposedPropertyTable Resolver { get; set; }
        public object TrackBinding { get; set; }
        public ClipCaps clipCaps => ClipCaps.Blending;


        public abstract void InitializeClip(IExposedPropertyTable resolver);


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph, (T) Template);
            Clone = playable.GetBehaviour();
           
            Clone.TimelineClip = TimelineClip;
            Clone.TrackBinding = TrackBinding;
            Clone.Initialize();
            
            InitializeClip(Resolver);
            

            return playable;
        }
    }


    public interface IClip
    {
        TimelineClip TimelineClip { get; set; }
        IExposedPropertyTable Resolver { get; set; }
        object TrackBinding { get; set; }
    }
}