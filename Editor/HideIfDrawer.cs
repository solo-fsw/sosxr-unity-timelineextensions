using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldHide(property))
            {
                return 0;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ShouldHide(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }


        private bool ShouldHide(SerializedProperty property)
        {
            var hideIf = (HideIfAttribute) attribute;
            var conditionProperty = property.serializedObject.FindProperty(hideIf.ConditionProperty);

            if (conditionProperty == null)
            {
                return false;
            }

            if (hideIf.CompareValue == null) // Boolean check
            {
                return conditionProperty.propertyType == SerializedPropertyType.Boolean && conditionProperty.boolValue;
            }

            switch (conditionProperty.propertyType)
            {
                case SerializedPropertyType.Enum:
                    var enumValue = conditionProperty.enumValueIndex;

                    return enumValue.Equals((int) hideIf.CompareValue);
                case SerializedPropertyType.Integer:
                    return conditionProperty.intValue.Equals(hideIf.CompareValue);
                case SerializedPropertyType.Float:
                    return conditionProperty.floatValue.Equals(hideIf.CompareValue);
                case SerializedPropertyType.Boolean:
                    return conditionProperty.boolValue.Equals(hideIf.CompareValue);
            }

            return false;
        }
    }
}