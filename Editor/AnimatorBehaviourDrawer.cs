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

            if (clip == null || clip.Template == null || clip.StateNames == null || clip.StateNames.Count == 0)
            {
                return;
            }

            int index = Mathf.Max(0, clip.StateNames.IndexOf(clip.Template.StateName));
            index = EditorGUI.Popup(position, "State", index, clip.StateNames.ToArray());
            clip.Template.StateName = clip.StateNames[index];
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
