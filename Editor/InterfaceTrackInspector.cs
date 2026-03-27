using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions.EditorScripts
{
    [CustomEditor(typeof(InterfaceTrack))]
    public class InterfaceTrackInspector : UnityEditor.Editor
    {
        private InterfaceTrack _track;

        private void OnEnable()
        {
            _track = target as InterfaceTrack;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var director = TimelineEditor.inspectedDirector;

            if (director == null)
            {
                EditorGUILayout.HelpBox("Open this track in the Timeline window to manage its binding.", MessageType.Info);
                serializedObject.ApplyModifiedProperties();

                return;
            }

            var currentBinding = director.GetGenericBinding(_track) as Component;

            EditorGUILayout.LabelField("Interface Track", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (currentBinding != null)
            {
                var prevColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.4f, 0.85f, 0.4f);
                EditorGUILayout.HelpBox($"✓  {currentBinding.GetType().Name}  on  '{currentBinding.gameObject.name}'", MessageType.None);
                GUI.backgroundColor = prevColor;

                EditorGUILayout.Space();

                if (GUILayout.Button("Clear Binding"))
                {
                    Undo.RecordObject(director, "Clear IInterface Binding");
                    director.SetGenericBinding(_track, null);
                    EditorUtility.SetDirty(director);
                    TimelineEditor.Refresh(RefreshReason.ContentsModified);
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"No {nameof(IInterface)} component bound. Use the button below to pick one.", MessageType.Warning);

                EditorGUILayout.Space();

                if (GUILayout.Button($"Pick {nameof(IInterface)} Component\u2026"))
                {
                    ShowPicker(director, currentBinding);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowPicker(UnityEngine.Playables.PlayableDirector director, Component currentBinding)
        {
            var implementors = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID)
                               .OfType<IInterface>()
                               .Cast<Component>()
                               .ToArray();

            var menu = new GenericMenu();

            if (implementors.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent($"No {nameof(IInterface)} components found in scene"));
            }
            else
            {
                foreach (var impl in implementors)
                {
                    var captured = impl;
                    var label = $"{impl.gameObject.name}/{impl.GetType().Name}";
                    var isSelected = impl == currentBinding;

                    menu.AddItem(new GUIContent(label), isSelected, () =>
                    {
                        Undo.RecordObject(director, "Set IInterface Binding");
                        director.SetGenericBinding(_track, captured);
                        EditorUtility.SetDirty(director);
                        TimelineEditor.Refresh(RefreshReason.ContentsModified);
                    });
                }
            }

            menu.ShowAsContext();
        }
    }
}
