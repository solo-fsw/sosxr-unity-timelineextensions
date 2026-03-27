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

            var currentBinding = director.GetGenericBinding(_track);

            EditorGUILayout.LabelField("Interface Track", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (currentBinding != null)
            {
                string location = currentBinding is Component c ? $"on '{c.gameObject.name}'" : AssetDatabase.GetAssetPath(currentBinding);

                var prevColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.4f, 0.85f, 0.4f);
                EditorGUILayout.HelpBox($"✓  {currentBinding.GetType().Name}  {location}", MessageType.None);
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
                EditorGUILayout.HelpBox($"No {nameof(IInterface)} bound. Use the button below to pick one.", MessageType.Warning);

                EditorGUILayout.Space();

                if (GUILayout.Button($"Pick {nameof(IInterface)}\u2026"))
                {
                    ShowPicker(director, currentBinding);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowPicker(UnityEngine.Playables.PlayableDirector director, UnityEngine.Object currentBinding)
        {
            var sceneImplementors = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID)
                                    .OfType<IInterface>()
                                    .Cast<Component>()
                                    .ToArray();

            var soGuids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}");
            var soImplementors = soGuids
                               .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
                               .Where(so => so is IInterface)
                               .ToArray();

            var menu = new GenericMenu();

            bool anyItem = false;

            if (sceneImplementors.Length != 0)
            {
                foreach (var impl in sceneImplementors)
                {
                    var captured = impl;
                    var label = $"Scene/{impl.gameObject.name}/{impl.GetType().Name}";
                    var isSelected = captured == currentBinding;
                    anyItem = true;

                    menu.AddItem(new GUIContent(label), isSelected, () =>
                    {
                        Undo.RecordObject(director, "Set IInterface Binding");
                        director.SetGenericBinding(_track, captured);
                        EditorUtility.SetDirty(director);
                        TimelineEditor.Refresh(RefreshReason.ContentsModified);
                    });
                }
            }

            if (soImplementors.Length != 0)
            {
                foreach (var so in soImplementors)
                {
                    var captured = so;
                    var path = AssetDatabase.GetAssetPath(captured);
                    var isSelected = captured == currentBinding;
                    anyItem = true;

                    menu.AddItem(new GUIContent($"Assets/{path}"), isSelected, () =>
                    {
                        Undo.RecordObject(director, "Set IInterface Binding");
                        director.SetGenericBinding(_track, captured);
                        EditorUtility.SetDirty(director);
                        TimelineEditor.Refresh(RefreshReason.ContentsModified);
                    });
                }
            }

            if (!anyItem)
            {
                menu.AddDisabledItem(new GUIContent($"No {nameof(IInterface)} found in scene or assets"));
            }

            menu.ShowAsContext();
        }
    }
}
