using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    public abstract class Track : TrackAsset
    {
        /// <summary>
        ///     Abstract method to get the binding type, e.g.:
        ///     return typeof(ExampleThing);
        /// </summary>
        /// <returns></returns>
        protected abstract Type GetBindingType();


        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var timelineClip in GetClips())
            {
                // Use type checking instead of direct casting
                if (timelineClip.asset is IClip clip)
                {
                    clip.InitializeClip(timelineClip);
                }
            }

            return CreateMixerPlayable(graph, inputCount);
        }


        /// <summary>
        ///     Abstract method to create the mixer playable, e.g.:
        ///     return ScriptPlayable<ExampleMixer>.Create(graph, inputCount);
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="inputCount"></param>
        /// <returns></returns>
        protected abstract Playable CreateMixerPlayable(PlayableGraph graph, int inputCount);


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
    }
}