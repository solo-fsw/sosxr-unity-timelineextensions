using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Base clip asset for all SOSXR Timeline Extension tracks.
    ///     Stores the track binding, resolver, and TimelineClip reference so derived classes can use them in CreatePlayable.
    ///     Always call base.InitializeClip() when overriding <see cref="InitializeClip"/>.
    /// </summary>
    [Serializable] // Also on the derived class, Clips need to be serializable
    public abstract class Clip : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        ///     This gets you information on the actual clip that's holding the Clip. Sorry, the naming is a little confusing.
        ///     Just note that this gets you information on the duration, easing times, playback speed, etc of the clip.
        /// </summary>
        public TimelineClip TimelineClip { get; private set; }

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
        public object TrackBinding { get; private set; }

        public virtual ClipCaps clipCaps => ClipCaps.Blending;

        // Cache the type name to avoid reflection overhead in error messages
        private string _cachedTypeName;

        /// <summary>
        ///     Returns the cached type name for this clip to use in debug messages.
        /// </summary>
        protected string TypeName => _cachedTypeName ??= GetType().Name;

        /// <summary>
        ///     This gets called when the Clip is created on the Track.
        ///     No need to call this manually, it gets called automatically on the Track.
        ///     Always call this base method when overriding this method.
        /// </summary>
        public virtual void InitializeClip(
            object trackBinding,
            TimelineClip timelineClip,
            IExposedPropertyTable resolver
        )
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
