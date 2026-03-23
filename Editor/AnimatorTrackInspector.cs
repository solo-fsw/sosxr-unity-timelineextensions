#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SOSXR.TimelineExtensions.EditorScripts
{
    [CustomEditor(typeof(AnimatorTrack))]
    public class AnimatorTrackInspector : UnityEditor.Editor
    {
        private AnimatorTrack _track;
        private Animator _animator;

        private void OnEnable()
        {
            _track = target as AnimatorTrack;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Animator Track", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_animator == null)
            {
                var director = UnityEditor.Timeline.TimelineEditor.inspectedDirector;
                if (director != null && _track != null)
                {
                    _animator = director.GetGenericBinding(_track) as Animator;
                }
            }

            if (_animator != null && _animator.runtimeAnimatorController != null)
            {
                EditorGUILayout.LabelField($"Bound Animator: {_animator.name}", EditorStyles.miniLabel);
                EditorGUILayout.Space();

                var states = _animator.GetStateNames();
                if (states != null && states.Count > 0)
                {
                    int currentIndex = Mathf.Max(0, states.IndexOf(_track.DefaultState));
                    int newIndex = EditorGUILayout.Popup("Default State", currentIndex, states.ToArray());

                    if (newIndex != currentIndex)
                    {
                        _track.DefaultState = states[newIndex];
                        EditorUtility.SetDirty(_track);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No states found in Animator Controller.", MessageType.Warning);
                    DrawDefaultProperty();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No Animator bound to this track. Bind an Animator in the Timeline window first.", MessageType.Info);
                DrawDefaultProperty();
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This is the state the Animator returns to when no clips are active.", MessageType.None);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDefaultProperty()
        {
            SerializedProperty defaultStateProp = serializedObject.FindProperty("DefaultState");
            if (defaultStateProp != null)
            {
                EditorGUILayout.PropertyField(defaultStateProp);
            }
        }
    }
}
#endif
