using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    /// <summary>
    ///     From: https://gist.github.com/LotteMakesStuff/c0a3b404524be57574ffa5f8270268ea
    ///     With additions from ChatGPT and Claude.
    ///     Limitations: Does not disable '+' button for lists.
    /// </summary>
    [CustomPropertyDrawer(typeof(DisableEditingAttribute))]
    public class DisableEditingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);

            EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndDisabledGroup();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}