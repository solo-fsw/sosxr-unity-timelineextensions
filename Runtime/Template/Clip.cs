using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable] // Also on the derived class, Clips need to be serializable
    public abstract class Clip : PlayableAsset, ITimelineClipAsset, IClip
    {
        /// <summary>
        ///     This gets you information on the actual clip that's holding the Clip. Sorry, the naming is a little confusing.
        ///     Just note that this gets you information on the duration, easing times, playback speed, etc of the clip.
        /// </summary>
        public TimelineClip TimelineClip { get; set; }

        /// <summary>
        ///     If you use ExposedReference<> in your Clip, you can use this to resolve it to it's underlying type.
        ///     You can do this in the InitializeClip method for instance.
        ///     Example: `clone.ExampleTransform = ExampleTransformReference.Resolve(Resolver);`
        /// </summary>
        public IExposedPropertyTable Resolver { get; set; }

        /// <summary>
        ///     Use this to get the object that the Track is bound to.
        ///     You usually want to cast it to the specific type of your binding.
        /// </summary>
        public object TrackBinding { get; set; }


        /// <summary>
        ///     This gets called when the Clip is created on the Track.
        ///     Useful for setting up anything in the Clip.
        ///     No need to call this manually, it gets called automatically on the Track.
        /// </summary>
        public virtual void InitializeClip()
        {
        }


        public virtual ClipCaps clipCaps => ClipCaps.Blending;


        /// <summary>
        ///     From here also call the InitializeBehaviour method of the Behaviour script. See the ExampleClip for an example.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public abstract override Playable CreatePlayable(PlayableGraph graph, GameObject owner);
    }


    public interface IClip
    {
        TimelineClip TimelineClip { get; set; }
        IExposedPropertyTable Resolver { get; set; }
        object TrackBinding { get; set; }


        void InitializeClip();
    }
}