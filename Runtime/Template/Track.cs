using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.0f, 0.1412f, 0.4902f)] // (0, 0.1412, 0.4902) is a dark blue, Leiden University's house colour
    public class Track : TrackAsset
    {
        #region Mandatory to Override in the Implementation

        /// <summary>
        ///     Method to get the binding type, used in the GatherProperties method for allowing Timeline to work outside PlayMode.
        ///     Usage example: `return typeof(ExampleThing);`
        /// </summary>
        /// <returns></returns>
        protected virtual Type GetBindingType()
        {
            return null;
        }


        /// <summary>
        ///     Method to create the Mixer of the Implementation.
        ///     Make sure you pass along the TrackBinding
        ///     See Readme for details
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="inputCount"></param>
        /// <returns></returns>
        protected virtual Playable CreateMixerPlayable(PlayableGraph graph, int inputCount)
        {
            return Playable.Null;
        }

        #endregion


        #region Other Things

        /// <summary>
        ///     I'm hoping on that this doesn't need to get overriden in the actual implementation, and that I've covered most
        ///     use-cases in the InitializeClip method of the Clip script.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="go"></param>
        /// <param name="inputCount"></param>
        /// <returns></returns>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            TrackBinding = go.GetComponent<PlayableDirector>().GetGenericBinding(this);
            var resolver = graph.GetResolver();

            foreach (var timelineClip in GetClips())
            {
                if (timelineClip.asset is IClip clip)
                {
                    clip.TimelineClip = timelineClip;
                    clip.Resolver = resolver;
                    clip.TrackBinding = TrackBinding;
                    clip.InitializeClip();
                }
            }

            return CreateMixerPlayable(graph, inputCount);
        }


        public Object TrackBinding { get; set; }


        /// <summary>
        ///     This is what allows the Preview to work in the Timeline Editor, without it changing the values in the Inspector.
        ///     It's creating a SerializedObject from the trackBinding and iterating through its properties to add them to the
        ///     driver. The driver then knows that these properties are being used by the Timeline, and don't need to be saved.
        /// </summary>
        /// <param name="director"></param>
        /// <param name="driver"></param>
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            #if UNITY_EDITOR
            var trackBinding = director.GetGenericBinding(this);

            if (trackBinding == null)
            {
                return;
            }

            var bindingType = GetBindingType();

            if (!bindingType.IsInstanceOfType(trackBinding))
            {
                return;
            }

            if (trackBinding is not Component component)
            {
                Debug.LogWarning("Track binding is not a component");

                return;
            }

            var serializedObject = new SerializedObject(trackBinding);
            var iterator = serializedObject.GetIterator();

            while (iterator.NextVisible(true))
            {
                if (iterator.hasVisibleChildren)
                {
                    continue;
                }

                AddPropertyToDriver(driver, component.gameObject, iterator.propertyPath, bindingType); // Call the appropriate AddFromName method using a helper method
            }

            #endif

            base.GatherProperties(director, driver);
        }


        #if UNITY_EDITOR
        /// <summary>
        ///     Helper method to add property to driver using the correct generic type
        ///     Find the specific AddFromName method with the correct parameters, then makes it generic and invokes it
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="gameObject"></param>
        /// <param name="propertyPath"></param>
        /// <param name="componentType"></param>
        private void AddPropertyToDriver(IPropertyCollector driver, GameObject gameObject, string propertyPath, Type componentType)
        {
            var method = typeof(IPropertyCollector).GetMethods()
                                                   .Where(m => m.Name == "AddFromName" && m.IsGenericMethod)
                                                   .Where(m =>
                                                   {
                                                       var parameters = m.GetParameters();

                                                       return parameters.Length == 2 &&
                                                              parameters[0].ParameterType == typeof(GameObject) &&
                                                              parameters[1].ParameterType == typeof(string);
                                                   })
                                                   .FirstOrDefault();

            if (method == null)
            {
                return;
            }

            var genericMethod = method.MakeGenericMethod(componentType);

            genericMethod.Invoke(driver, new object[] {gameObject, propertyPath});
        }
        #endif

        #endregion
    }
}