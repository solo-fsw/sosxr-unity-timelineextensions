using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorBehaviour))]
    public class AnimatorDrawer : PropertyDrawer
    {
        private SerializedProperty exposedReference;

        private readonly List<string> stateNames = new() {""};
        private Animator animator;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var clip = property.serializedObject.targetObject as AnimatorClip;

            if (clip == null)
            {
                return;
            }

            var clipTemplate = clip.Template;

            UpdateStateList(clipTemplate.TrackBinding);

            ClipStartFields(property, clipTemplate);

            ClipEndFields(property, clipTemplate);
        }


        private void UpdateStateList(Animator anim)
        {
            stateNames.Clear();
            stateNames.Add("NONE");

            if (anim.runtimeAnimatorController is not AnimatorController controller)
            {
                return;
            }

            foreach (var layer in controller.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    stateNames.Add(state.state.name);
                }
            }
        }


        private void ClipStartFields(SerializedProperty property, AnimatorBehaviour clipTemplate)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var stateNameProp = property.FindPropertyRelative(nameof(clipTemplate.StartClipStateName));

            var selectedStateIndex = stateNames.IndexOf(clipTemplate.StartClipStateName);

            var newSelectedStateIndex = EditorGUILayout.Popup("Clip Start State: ", selectedStateIndex, stateNames.ToArray());

            if (newSelectedStateIndex != selectedStateIndex)
            {
                clipTemplate.StartClipStateName = stateNames[newSelectedStateIndex];
                stateNameProp.stringValue = clipTemplate.StartClipStateName;
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();
        }


        private void ClipEndFields(SerializedProperty property, AnimatorBehaviour clipTemplate)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var stateNameProp = property.FindPropertyRelative(nameof(clipTemplate.EndClipStateName));

            var selectedStateIndex = stateNames.IndexOf(clipTemplate.EndClipStateName);

            var newSelectedStateIndex = EditorGUILayout.Popup("Clip End State: ", selectedStateIndex, stateNames.ToArray());

            if (newSelectedStateIndex != selectedStateIndex)
            {
                clipTemplate.EndClipStateName = stateNames[newSelectedStateIndex];
                stateNameProp.stringValue = clipTemplate.EndClipStateName;
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();
        }
    }
}