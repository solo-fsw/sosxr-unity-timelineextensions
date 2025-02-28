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

            /*if (stateNameProp.stringValue != "NONE")
            {
                var transitionDurationProp = property.FindPropertyRelative(nameof(clipTemplate.StartTransitionDuration));
                EditorGUILayout.Slider(transitionDurationProp, 0, 5, new GUIContent("Transition Duration"));

                var clipDurationProp = property.FindPropertyRelative(nameof(clipTemplate.ClipDuration));

                var endClipStateNameProp = property.FindPropertyRelative(nameof(clipTemplate.EndClipStateName));
                if (transitionDurationProp.floatValue > clipDurationProp.floatValue && endClipStateNameProp.stringValue != "NONE")
                {
                    EditorGUILayout.HelpBox("The duration of your crossfade into your start animation is longer than the duration of the clip. This can't be good, I think you'll get problems with your 'Clip End State' firing.", MessageType.Warning);
                }
            }*/

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

            /*
            if (stateNameProp.stringValue != "NONE")
            {
                var transitionDurationProp = property.FindPropertyRelative(nameof(clipTemplate.EndTransitionDuration));
                EditorGUILayout.Slider(transitionDurationProp, 0, 5, new GUIContent("Reset Transition Duration"));

                if (transitionDurationProp.floatValue > 0)
                {
                    EditorGUILayout.HelpBox("Your clip end animation will crossfade beyond the boundaries of the current clip. This is not a problem per se, but good to be aware of. It may give unexpected behaviour if another clip is trying to crossfade within the time that this one is still fading... maybe?", MessageType.Info);
                }
            }
            */

            EditorGUILayout.EndVertical();
        }
    }
}