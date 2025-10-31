using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable] // Also on the derived class, Clips need to be serializable
    public abstract class Clip : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        ///     This gets you information on the actual clip that's holding the Clip. Sorry, the naming is a little confusing.
        ///     Just note that this gets you information on the duration, easing times, playback speed, etc of the clip.
        /// </summary>
        protected TimelineClip TimelineClip { get; private set; }

        /// <summary>
        ///     If you use ExposedReference<> in your Clip, you can use this to resolve it to it's underlying type.
        ///     You can do this in the InitializeClip method for instance.
        ///     Example: `clone.ExampleTransform = ExampleTransformReference.Resolve(Resolver);`
        /// </summary>
        protected IExposedPropertyTable Resolver { get; private set; }

        /// <summary>
        ///     Use this to get the object that the Track is bound to.
        ///     You usually want to cast it to the specific type of your binding.
        /// </summary>
        protected object TrackBinding { get; private set; }


        public virtual ClipCaps clipCaps => ClipCaps.Blending;


        /// <summary>
        ///     This gets called when the Clip is created on the Track.
        ///     No need to call this manually, it gets called automatically on the Track.
        ///     Always call this base method when overriding this method.
        /// </summary>
        public virtual void InitializeClip(object trackBinding, TimelineClip timelineClip, IExposedPropertyTable resolver)
        {
            TrackBinding = trackBinding;
            TimelineClip = timelineClip;
            Resolver = resolver;
        }


        /// <summary>
        ///     From here also call the InitializeBehaviour method of the Behaviour script. See the ExampleClip for an example.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public abstract override Playable CreatePlayable(PlayableGraph graph, GameObject owner);
    }
}