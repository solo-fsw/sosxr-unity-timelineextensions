using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorBehaviour))]
    public class AnimatorDrawer : PropertyDrawer
    {
        private SerializedProperty _exposedReference;

        private readonly List<string> _stateNames = new() {""};
        private string _defaultStateName = "";


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var clip = property.serializedObject.targetObject as AnimatorClip;

            if (clip == null)
            {
                Debug.LogWarning("Clip is null");
                return;
            }

            if (clip.BehaviourTemplate is not AnimatorBehaviour clipTemplate)
            {
                Debug.LogWarning("Clip template is null");
                return;
            }

            var trackBinding = (Animator) clipTemplate.TrackBinding;

            UpdateStateList(trackBinding);

            ClipStartFields(property, clipTemplate);

            ClipEndFields(property, clipTemplate);
        }


        private void UpdateStateList(Animator anim)
        {
            _stateNames.Clear();
            _stateNames.Add("NONE");

            if (anim == null)
            {
                Debug.LogWarning("Animator is null");

                return;
            }

            if (anim.runtimeAnimatorController is not AnimatorController controller)
            {
                Debug.LogWarning("Animator controller is null");
                return;
            }

            foreach (var layer in controller.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    _stateNames.Add(state.state.name);
                }
            }
        }


        private void ClipStartFields(SerializedProperty property, AnimatorBehaviour clipTemplate)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var stateNameProp = property.FindPropertyRelative(nameof(clipTemplate.StartClipStateName));

            var selectedStateIndex = _stateNames.IndexOf(clipTemplate.StartClipStateName);

            var newSelectedStateIndex = EditorGUILayout.Popup("Animation State when clip STARTS: ", selectedStateIndex, _stateNames.ToArray());

            if (newSelectedStateIndex != selectedStateIndex)
            {
                clipTemplate.StartClipStateName = _stateNames[newSelectedStateIndex];
                stateNameProp.stringValue = clipTemplate.StartClipStateName;
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();
        }


        private void ClipEndFields(SerializedProperty property, AnimatorBehaviour clipTemplate)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var stateNameProp = property.FindPropertyRelative(nameof(clipTemplate.EndClipStateName));

            if (stateNameProp.stringValue == "Default_State")
            {
                var anim = clipTemplate.TrackBinding as Animator;

                if (anim == null)
                {
                    return;
                }

                var controller = anim.runtimeAnimatorController as AnimatorController;

                if (controller != null)
                {
                    _defaultStateName = GetDefaultEntryStateName(controller);
                }

                clipTemplate.EndClipStateName = _defaultStateName;
                stateNameProp.stringValue = clipTemplate.EndClipStateName;
                property.serializedObject.ApplyModifiedProperties();
            }

            var selectedStateIndex = _stateNames.IndexOf(clipTemplate.EndClipStateName);

            var newSelectedStateIndex = EditorGUILayout.Popup("Animation State when clip ENDS: ", selectedStateIndex, _stateNames.ToArray());

            if (newSelectedStateIndex != selectedStateIndex)
            {
                clipTemplate.EndClipStateName = _stateNames[newSelectedStateIndex];
                stateNameProp.stringValue = clipTemplate.EndClipStateName;
                property.serializedObject.ApplyModifiedProperties();
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