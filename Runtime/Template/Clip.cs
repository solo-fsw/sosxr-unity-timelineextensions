using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    public abstract class Clip : PlayableAsset, ITimelineClipAsset
    {
        protected TimelineClip TimelineClip { get; private set; }
        public ClipCaps clipCaps => ClipCaps.Blending;

        /// <summary>
        ///     Abstract method to get the binding type, e.g.:
        ///     return typeof(ExampleThing);
        /// </summary>
        /// <returns></returns>
        protected abstract Type GetBindingType();

        
        public void Initialize(TimelineClip timelineClip)
        {
            TimelineClip = timelineClip;
        }
    }
}