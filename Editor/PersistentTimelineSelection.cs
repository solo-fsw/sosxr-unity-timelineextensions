using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions.EditorScripts
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
        private const string SessionStateKey = "SOSXR.PersistentTimeline.LastDirectorInstanceID";

        static PersistentTimelineSelection()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static TimelineEditorWindow _timelineEditorWindow;

        private static PlayableDirector LastSelectedDirector
        {
            get
            {
                var instanceID = SessionState.GetInt(SessionStateKey, 0);
                if (instanceID == 0)
                    return null;
                return UnityEditor.EditorUtility.EntityIdToObject(instanceID) as PlayableDirector;
            }
            set
            {
                if (value == null)
                {
                    SessionState.EraseInt(SessionStateKey);
                }
                else
                {
                    SessionState.SetInt(SessionStateKey, value.GetInstanceID());
                }
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                var director = LastSelectedDirector;
                if (director != null && TryGetTimelineWindow())
                {
                    _timelineEditorWindow.SetTimeline(director);
                }
            }
        }

        private static void OnSelectionChanged()
        {
            if (!TryGetTimelineWindow())
            {
                return;
            }

            PlayableDirector currentlySelectedPlayableDirector = null;

            if (Selection.activeGameObject != null)
            {
                currentlySelectedPlayableDirector =
                    Selection.activeGameObject.GetComponent<PlayableDirector>();
            }

            if (currentlySelectedPlayableDirector != null)
            {
                LastSelectedDirector = currentlySelectedPlayableDirector;
            }
            else
            {
                var lastDirector = LastSelectedDirector;
                if (lastDirector != null)
                {
                    _timelineEditorWindow.SetTimeline(lastDirector);
                }
            }
        }

        private static bool TryGetTimelineWindow()
        {
            if (_timelineEditorWindow != null)
            {
                return true;
            }

            _timelineEditorWindow = TimelineEditor.GetWindow(); // Alternative: TimelineEditor.GetOrCreateWindow();

            return _timelineEditorWindow != null;
        }
    }
}
