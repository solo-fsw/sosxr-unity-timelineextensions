using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
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

        #region Useful to override in the implementation

        /// <summary>
        ///     This gets called when the Clip is created on the Track.
        /// </summary>
        public virtual void InitializeClip()
        {
             Debug.Log("Clip initialized by the Track");
        }

        #endregion

        public ClipCaps clipCaps => ClipCaps.Blending;


        #region Mandatory to override in the implementation

        /// <summary>
        ///     From here also call the InitializeBehaviour method of the Behaviour script. See the ExampleClip for an example.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public abstract override Playable CreatePlayable(PlayableGraph graph, GameObject owner);

        #endregion
    }


    public interface IClip
    {
        TimelineClip TimelineClip { get; set; }
        IExposedPropertyTable Resolver { get; set; }
        object TrackBinding { get; set; }


        void InitializeClip();
    }
}