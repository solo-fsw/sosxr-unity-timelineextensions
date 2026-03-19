using UnityEditor;
using UnityEngine;

namespace SOSXR.TimelineExtensions.EditorScripts
{
    [CustomPropertyDrawer(typeof(AnimatorBehaviour))]
    public class AnimatorBehaviourDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            AnimatorClip clip = property.serializedObject.targetObject as AnimatorClip;

            if (clip == null)
            {
                EditorGUI.LabelField(position, "AnimatorClip not found");

                return;
            }

            var clipTemplate = clip.Template;

            var stateNames = clip.StateNames;

            if (clipTemplate == null)
            {
                return;
            }

            if (stateNames == null || stateNames.Count == 0)
            {
                return;
            }

            int startIndex = Mathf.Max(0, clip.StateNames.IndexOf(clipTemplate.StartClipStateName));
            int endIndex = Mathf.Max(0, clip.StateNames.IndexOf(clipTemplate.EndClipStateName));

            position.height = EditorGUIUtility.singleLineHeight;
            startIndex = EditorGUI.Popup(position, "Start State", startIndex, stateNames.ToArray());
            position.y += EditorGUIUtility.singleLineHeight + 2;
            endIndex = EditorGUI.Popup(position, "End State", endIndex, stateNames.ToArray());

            clipTemplate.StartClipStateName = stateNames[startIndex];
            clipTemplate.EndClipStateName = stateNames[endIndex];

            if (GUILayout.Button("Match Clip To StartState Duration"))
            {
                clip.MatchClipToStartStateDuration();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => (EditorGUIUtility.singleLineHeight + 2) * 2;
    }
}
