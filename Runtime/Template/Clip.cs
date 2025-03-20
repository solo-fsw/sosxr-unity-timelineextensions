using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class Clip : PlayableAsset, ITimelineClipAsset, IClip
    {
        public TimelineClip TimelineClip { get; set; }
        public IExposedPropertyTable Resolver { get; set; }
        public object TrackBinding { get; set; }


        /// <summary>
        ///     This gets called when the Clip is created on the Track.
        /// </summary>
        public virtual void InitializeClip()
        {
        }


        public ClipCaps clipCaps => ClipCaps.Blending;


        /// <summary>
        ///     From here also call the InitializeBehaviour method of the Behaviour script, from when you've created a Behaviour as
        ///     a clone
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Null;
        }
    }


    public interface IClip
    {
        TimelineClip TimelineClip { get; set; }
        IExposedPropertyTable Resolver { get; set; }
        object TrackBinding { get; set; }


        void InitializeClip();
    }
}