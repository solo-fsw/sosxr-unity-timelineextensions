using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;


namespace UnityDefaultPlayables
{
    [TrackColor(0.875f, 0.5944853f, 0.1737132f)]
    [TrackClipType(typeof(ScreenFaderClip))]
    [TrackBindingType(typeof(Image))]
    public class ScreenFaderTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ScreenFaderMixerBehaviour>.Create(graph, inputCount);
        }


        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            #if UNITY_EDITOR
            var trackBinding = director.GetGenericBinding(this) as Image;

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

                driver.AddFromName<Image>(trackBinding.gameObject, iterator.propertyPath);
            }
            #endif
            base.GatherProperties(director, driver);
        }
    }
}