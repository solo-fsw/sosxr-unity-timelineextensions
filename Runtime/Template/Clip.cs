using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public  class Clip : PlayableAsset, ITimelineClipAsset, IClip
    {
        //public Behaviour Template { get; private set; }
        
        public TimelineClip TimelineClip { get; set; }
        public IExposedPropertyTable Resolver { get; set; }
        public object TrackBinding { get; set; }
        public ClipCaps clipCaps => ClipCaps.Blending;

        


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            /*var playable = ScriptPlayable<Behaviour>.Create(graph, Template);
            Clone = playable.GetBehaviour();
           
            Clone.TimelineClip = TimelineClip;
            Clone.TrackBinding = TrackBinding;
            Clone.InitializeBehaviour();
            
            InitializeClip(Resolver);
            
            return playable;*/
            
            return Playable.Null;
        }
    }


    public interface IClip
    {
        TimelineClip TimelineClip { get; set; }
        IExposedPropertyTable Resolver { get; set; }
        object TrackBinding { get; set; }
    }
}