using UnityEditor;
using UnityEngine;


namespace UnityDefaultPlayables.Editor
{
    /// <summary>
    /// By Unity: https://assetstore.unity.com/packages/tools/default-playables-95266#content
    /// </summary>
    [CustomPropertyDrawer(typeof(ScreenFaderBehaviour))]
    public class ScreenFaderDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            int fieldCount = 1;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty colorProp = property.FindPropertyRelative("color");

            Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, colorProp);
        }
    }
}
