using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    public abstract class Clip<T> : PlayableAsset, ITimelineClipAsset, IClip where T : Behaviour, new()
    {
        public Behaviour Template;
        public Behaviour Clone;
        public TimelineClip TimelineClip { get; set; }
        public IExposedPropertyTable Resolver { get; set; }


        public abstract void InitializeClip(IExposedPropertyTable resolver);


        public ClipCaps clipCaps => ClipCaps.Blending;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph, (T) Template);
            Clone = playable.GetBehaviour();
            Clone.TimelineClip = TimelineClip;
            
            InitializeClip(Resolver);
       
            return playable;
        }
    }


    public interface IClip
    {
        TimelineClip TimelineClip { get; set; }
        IExposedPropertyTable Resolver { get; set; }


        void InitializeClip(IExposedPropertyTable resolver);
    }
}