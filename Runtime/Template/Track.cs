using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.855f, 0.8623f, 0.870f)]
    [TrackClipType(typeof(Clip))] 
    [TrackBindingType(typeof(Transform))] // Change binding here
    public class Track : TLTrack
    {
    
    }


    public class TLTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var timelineClip in GetClips())
            {
                if (timelineClip.asset is not TLClip clip)
                {
                    continue;
                }

                clip.Initialize(timelineClip);
            }

            var mixer = ScriptPlayable<Mixer>.Create(graph, inputCount);

            return mixer;
        }


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
            var trackBinding = director.GetGenericBinding(this) as Transform; // Change binding here

            if (trackBinding == null)
            {
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

                driver.AddFromName<Transform>(trackBinding.gameObject, iterator.propertyPath); // Change binding here
            }
            #endif

            base.GatherProperties(director, driver);
        }
    }
}