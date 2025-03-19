using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    public abstract class TLClip : PlayableAsset, ITimelineClipAsset
    {
        protected TimelineClip TimelineClip { get; private set; }
        public ClipCaps clipCaps => ClipCaps.Blending;


        public void Initialize(TimelineClip timelineClip)
        {
            TimelineClip = timelineClip;
        }
    }
}