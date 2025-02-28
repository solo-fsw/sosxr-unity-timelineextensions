using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    [CustomPropertyDrawer(typeof(TLActivateBehaviour))]
    public class TLActivateDrawer : PropertyDrawer
    {
        private SerializedProperty _exposedReference;
        private const string WarningMessage = "The 'Activate at End' is also triggered when the behaviour is paused!";


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var clip = property.serializedObject.targetObject as TLActivateClip;

            if (!clip)
            {
                return;
            }

            var clipTemplate = clip.template;

            if (clipTemplate == null)
            {
                return;
            }

            _exposedReference ??= property.FindPropertyRelative(nameof(clipTemplate.ActivateReference));

            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(_exposedReference, new GUIContent("Activate"));
            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            clipTemplate.ActivateAtStart = EditorGUILayout.ToggleLeft("Trigger Activate clip start", clipTemplate.ActivateAtStart);
            clipTemplate.ActivateAtEnd = EditorGUILayout.ToggleLeft("Trigger Activate clip end", clipTemplate.ActivateAtEnd);
            GUILayout.EndVertical();

            if (clipTemplate.ActivateAtEnd)
            {
                EditorGUILayout.Space(15f);
                EditorGUILayout.HelpBox(WarningMessage, MessageType.Warning, true);
            }
        }
    }
}