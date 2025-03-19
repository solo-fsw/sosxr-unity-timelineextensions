using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    public abstract class Clip<T> : PlayableAsset, ITimelineClipAsset, IClip where T : Behaviour, new()
    {
        public Behaviour BehaviourTemplate;
        protected Behaviour GenericBehaviourImplementation { get; private set; }
        public TimelineClip TimelineClip { get; set; }
        public IExposedPropertyTable Resolver { get; set; }
        public ClipCaps clipCaps => ClipCaps.Blending;

        public abstract void InitializeClip(IExposedPropertyTable resolver);


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph, (T) BehaviourTemplate);
            GenericBehaviourImplementation = playable.GetBehaviour();
            GenericBehaviourImplementation.TimelineClip = TimelineClip;

            InitializeClip(Resolver);

            return playable;
        }
    }


    public interface IClip
    {
        TimelineClip TimelineClip { get; set; }
        IExposedPropertyTable Resolver { get; set; }
    }
}