using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    [CustomPropertyDrawer(typeof(TimeControlBehaviour))]
    public class TimeControlBehaviourDrawer : PropertyDrawer
    {
        private SerializedProperty exposedReference;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var clip = property.serializedObject.targetObject as TimeControlClip;

            if (!clip)
            {
                return;
            }

            var clipTemplate = clip.behaviour;

            exposedReference ??= property.FindPropertyRelative(nameof(clipTemplate.LoopBreakerReference));

            DrawLoopBreaker();

            DrawLooperVariables(clipTemplate);
        }


        private void DrawLoopBreaker()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.PropertyField(exposedReference, new GUIContent("Loop Breaker"));

            GUILayout.EndVertical();
        }


        private static void DrawLooperVariables(TimeControlBehaviour clipTemplate)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            clipTemplate.InitialState = (TimeState) EditorGUILayout.EnumPopup("State at start", clipTemplate.InitialState);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("Current state", clipTemplate.CurrentState.ToString());
            }

            GUILayout.EndVertical();
        }
    }
}