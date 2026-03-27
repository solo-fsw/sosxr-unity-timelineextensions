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

            if (binding != null && binding is not IInterface)
            {
                options.errorText = $"Bound component must implement {nameof(IInterface)}";
            }

            return options;
        }
    }
}
