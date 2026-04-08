using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Base track asset for all SOSXR Timeline Extension tracks.
    ///     Initializes each clip with the track binding and resolver, then delegates mixer creation to the derived class.
    ///     Colored with Leiden University's house blue by default.
    /// </summary>
    [Serializable]
    [TrackColor(0.0f, 0.17f, 0.88f)] // 't is a dark blue, Leiden University's house colour
    public abstract class Track : TrackAsset
    {
        /// <summary>
        ///     Use this to get the object that the Track is bound to.
        ///     You usually want to cast it to the specific type of your binding.
        /// </summary>
        protected object TrackBinding { get; set; }

        /// <summary>
        ///     If you use ExposedReference<> in your Clip, you can use this to resolve it to it's underlying type.
        ///     You can do this in the InitializeClip method for instance.
        ///     Example: `clone.ExampleTransform = ExampleTransformReference.Resolve(Resolver);`
        /// </summary>
        protected IExposedPropertyTable Resolver { get; private set; }

        public Mixer Mixer { get; private set; }


        /// <summary>
        ///     Method to create the Mixer of the Implementation.
        ///     Make sure you pass along the TrackBinding
        ///     See Readme for details
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="inputCount"></param>
        /// <returns></returns>
        protected abstract Playable CreateMixer(PlayableGraph graph, int inputCount);


        /// <summary>
        ///     I'm hoping on that this doesn't need to get overriden in the actual implementation, and that I've covered most
        ///     use-cases in the InitializeClip method of the Clip script.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="go"></param>
        /// <param name="inputCount"></param>
        /// <returns></returns>
        public sealed override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            // Guard against null PlayableDirector to prevent NullReferenceException
            var director = go.GetComponent<PlayableDirector>();
            if (director == null)
            {
                Debug.LogError($"{GetType().Name}: No PlayableDirector found on the GameObject. Cannot create track mixer.");
                return Playable.Null;
            }
            TrackBinding = director.GetGenericBinding(this);

            bool requiresBinding = GetType().GetCustomAttributes(typeof(NoTrackBindingAttribute), true).Length == 0;

            if (requiresBinding && TrackBinding == null)
            {
                Debug.LogWarning($"{GetType().Name}: There is nothing bound to this Track. Did you forget to set it?");
            }

            Resolver = graph.GetResolver();

            foreach (var timelineClip in GetClips())
            {
                if (timelineClip.asset is Clip clip)
                {
                    clip.InitializeClip(TrackBinding, timelineClip, Resolver);
                }
            }

            return CreateMixer(graph, inputCount);
        }
    }
}
