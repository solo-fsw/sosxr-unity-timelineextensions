using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    [CustomPropertyDrawer(typeof(AnimatorBehaviour))]
    public class AnimatorBehaviourDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var clip = property.serializedObject.targetObject as AnimatorClip;

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

            var startIndex = Mathf.Max(0, clip.StateNames.IndexOf(clipTemplate.StartClipStateName));
            var endIndex = Mathf.Max(0, clip.StateNames.IndexOf(clipTemplate.EndClipStateName));

            position.height = EditorGUIUtility.singleLineHeight;
            startIndex = EditorGUI.Popup(position, "Start State", startIndex, stateNames.ToArray());
            position.y += EditorGUIUtility.singleLineHeight + 2;
            endIndex = EditorGUI.Popup(position, "End State", endIndex, stateNames.ToArray());

            clipTemplate.StartClipStateName = stateNames[startIndex];
            clipTemplate.EndClipStateName = stateNames[endIndex];
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * 2;
        }
    }
}