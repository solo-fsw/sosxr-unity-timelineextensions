using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public abstract class Clip<T> : PlayableAsset, ITimelineClipAsset, IClip where T : Behaviour, new()
    {
        public Behaviour BehaviourTemplate { get; private set; }
        public Behaviour GenericBehaviourImplementation { get; private set; }
        public TimelineClip TimelineClip { get; set; }
        public IExposedPropertyTable Resolver { get; set; }
        public object TrackBinding { get; set; }
        public ClipCaps clipCaps => ClipCaps.Blending;


        public abstract void InitializeClip(IExposedPropertyTable resolver);


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph, (T) BehaviourTemplate);
            GenericBehaviourImplementation = playable.GetBehaviour();
            GenericBehaviourImplementation.TimelineClip = TimelineClip;
            GenericBehaviourImplementation.TrackBinding = TrackBinding;
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