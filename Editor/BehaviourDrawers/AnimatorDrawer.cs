using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorBehaviour))]
    public class AnimatorDrawer : PropertyDrawer
    {
        private SerializedProperty _exposedReference;
        private SerializedProperty _startProp;
        private SerializedProperty _endProp;
        private SerializedProperty _stateNamesProp;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            _startProp = property.FindPropertyRelative("StartClipStateName");
            _endProp = property.FindPropertyRelative("EndClipStateName");
            _stateNamesProp = property.FindPropertyRelative("StateNames");
            
            if (_startProp == null || _endProp == null)
            {
                EditorGUI.EndProperty();

                return;
            }

            var clip = property.serializedObject.targetObject as AnimatorClip;

            if (clip == null)
            {
                Debug.LogWarning("Clip is null");
                EditorGUI.EndProperty();

                return;
            }

            var clipTemplate = clip.AnimatorTemplate;

            if (clipTemplate == null)
            {
                Debug.LogWarning("Clip template is null");
                EditorGUI.EndProperty();

                return;
            }


            EditorGUI.indentLevel++;

            ClipStartFields();
            ClipEndFields();

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }


        private void ClipStartFields()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            var selectedStateIndex = _stateNamesProp.FindPropertyRelative("Array.size").intValue;

            if (selectedStateIndex < 0)
            {
                selectedStateIndex = 0;
            }

            var stateNames = new string[_stateNamesProp.arraySize];

            for (var i = 0; i < _stateNamesProp.arraySize; i++)
            {
                stateNames[i] = _stateNamesProp.GetArrayElementAtIndex(i).stringValue;
            }

            var newSelectedStateIndex = EditorGUILayout.Popup("Animation State when clip STARTS: ", selectedStateIndex, stateNames);

            if (newSelectedStateIndex != selectedStateIndex)
            {
                _stateNamesProp.FindPropertyRelative("Array.size").intValue = newSelectedStateIndex;
                _stateNamesProp.serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();
        }


        private void ClipEndFields()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var selectedStateIndex = _stateNamesProp.FindPropertyRelative("Array.size").intValue;

            if (selectedStateIndex < 0)
            {
                selectedStateIndex = 0;
            }

            var stateNames = new string[_stateNamesProp.arraySize];

            for (var i = 0; i < _stateNamesProp.arraySize; i++)
            {
                stateNames[i] = _stateNamesProp.GetArrayElementAtIndex(i).stringValue;
            }

            var newSelectedStateIndex = EditorGUILayout.Popup("Animation State when clip ENDS: ", selectedStateIndex, stateNames);

            if (newSelectedStateIndex != selectedStateIndex)
            {
                _stateNamesProp.FindPropertyRelative("Array.size").intValue = newSelectedStateIndex;
                _stateNamesProp.serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();
        }


        public static string GetDefaultEntryStateName(AnimatorController controller)
        {
            if (controller == null)
            {
                Debug.LogWarning("Invalid animator controller");

                return "";
            }

            var stateMachine = controller.layers[0].stateMachine;

            return stateMachine.defaultState.name;
        }
    }
}