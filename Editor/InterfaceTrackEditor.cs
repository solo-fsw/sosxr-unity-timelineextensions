using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions.EditorScripts
{
    [CustomTimelineEditor(typeof(InterfaceTrack))]
    public class InterfaceTrackEditor : TrackEditor
    {
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            var options = base.GetTrackOptions(track, binding);

            var director = TimelineEditor.inspectedDirector;
            var actualBinding = director != null ? director.GetGenericBinding(track) : null;

            if (actualBinding != null && actualBinding is not IInterface)
            {
                options.errorText = $"Bound component must implement {nameof(IInterface)}";
            }

            return options;
        }
    }
}
