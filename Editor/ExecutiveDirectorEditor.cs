using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    [CustomEditor(typeof(ExecutiveDirector))]
    public class ExecutiveDirectorEditor : UnityEditor.Editor
    {
        private SerializedProperty m_autoPlayProp;
        private SerializedProperty m_durationDirectorsProp;
        private SerializedProperty m_totalDurationProp;


        private void OnEnable()
        {
            m_autoPlayProp = serializedObject.FindProperty("m_autoPlay");
            m_durationDirectorsProp = serializedObject.FindProperty("m_durationDirectors");
            m_totalDurationProp = serializedObject.FindProperty("m_totalDuration");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            if (!Application.isPlaying)
            {
                EditorGUILayout.PropertyField(m_autoPlayProp);
            }

            EditorGUILayout.Space();

            EditorGUI.indentLevel++;

            for (var i = 0; i < m_durationDirectorsProp.arraySize; i++)
            {
                var element = m_durationDirectorsProp.GetArrayElementAtIndex(i);
                var directorProp = element.FindPropertyRelative("Director");
                var isPlayingProp = element.FindPropertyRelative("IsPlaying");
                var durationProp = element.FindPropertyRelative("Duration");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(isPlayingProp, GUIContent.none, GUILayout.Width(20));
                EditorGUILayout.PropertyField(directorProp, GUIContent.none);
                EditorGUILayout.LabelField($"{durationProp.floatValue} s", GUILayout.Width(60));

                if (!Application.isPlaying)
                {
                    // Move up
                    GUI.enabled = i > 0;

                    if (GUILayout.Button("↑", GUILayout.Width(25)))
                    {
                        m_durationDirectorsProp.MoveArrayElement(i, i - 1);
                    }

                    // Move down
                    GUI.enabled = i < m_durationDirectorsProp.arraySize - 1;

                    if (GUILayout.Button("↓", GUILayout.Width(25)))
                    {
                        m_durationDirectorsProp.MoveArrayElement(i, i + 1);
                    }

                    GUI.enabled = true;

                    // Remove
                    if (GUILayout.Button("-", GUILayout.Width(25)))
                    {
                        m_durationDirectorsProp.DeleteArrayElementAtIndex(i);

                        break; // Prevent issues with modified list
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (!Application.isPlaying && GUILayout.Button("+ Add Director"))
            {
                m_durationDirectorsProp.InsertArrayElementAtIndex(m_durationDirectorsProp.arraySize);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"Total duration: {m_totalDurationProp.floatValue} seconds");

            EditorGUILayout.Space(10);

            if (Application.isPlaying && GUILayout.Button("Play All Directors"))
            {
                ((ExecutiveDirector) target).PlayAllDirectors();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}