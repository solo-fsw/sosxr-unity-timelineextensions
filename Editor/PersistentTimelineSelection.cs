using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Playables;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     This class will A) Display the currently selected PlayableDirector when the window is opened, and B) Remember the
    ///     last selected PlayableDirector and reselect it when a GameObject is selected which does not have a
    ///     PlayableDirector.
    ///     Unity's default behavior (to clear the PlayableDirector from the window when a non-Timeline GameObject is selected)
    ///     is quite annoying, and contrary to the behavior of other systems, like the Animator Window.
    /// </summary>
    [InitializeOnLoad]
    public static class PersistentTimelineSelection
    {
        static PersistentTimelineSelection()
        {
            // Debug.Log("SOSXR: PersistentTimelineSelection initialized. The Timeline Editor window will now remember the last selected PlayableDirector.");

            Selection.selectionChanged -= OnSelectionChanged; // Prevent duplicate subscriptions
            Selection.selectionChanged += OnSelectionChanged;
        }


        private static PlayableDirector lastSelectedDirector;
        private static TimelineEditorWindow timelineEditorWindow;


        private static void OnSelectionChanged()
        {
            if (!TryGetTimelineWindow())
            {
                return;
            }

            PlayableDirector currentlySelectedPlayableDirector = null;

            if (Selection.activeGameObject != null)
            {
                currentlySelectedPlayableDirector = Selection.activeGameObject.GetComponent<PlayableDirector>();
            }

            if (currentlySelectedPlayableDirector != null)
            {
                lastSelectedDirector = currentlySelectedPlayableDirector;
            }
            else if (lastSelectedDirector != null)
            {
                timelineEditorWindow.SetTimeline(lastSelectedDirector);
            }
        }


        private static bool TryGetTimelineWindow()
        {
            if (timelineEditorWindow != null)
            {
                return true;
            }

            timelineEditorWindow = TimelineEditor.GetWindow(); // Alternative: TimelineEditor.GetOrCreateWindow();

            return timelineEditorWindow != null;
        }
    }
}