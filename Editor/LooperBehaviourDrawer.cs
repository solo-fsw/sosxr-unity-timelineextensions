using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(LooperBehaviour))]
public class LooperBehaviourDrawer : PropertyDrawer
{
    private SerializedProperty exposedReference;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var clip = property.serializedObject.targetObject as LooperClip;

        if (!clip)
        {
            return;
        }

        var clipTemplate = clip.behaviour;

        exposedReference ??= property.FindPropertyRelative(nameof(clipTemplate.LoopBreakerObjectReference));

        DrawLoopBreaker();

        DrawLooperVariables(clipTemplate);
    }


    private void DrawLoopBreaker()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(exposedReference, new GUIContent("Loop Breaker"));

        GUILayout.EndVertical();
    }


    private static void DrawLooperVariables(LooperBehaviour clipTemplate)
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);

        clipTemplate.StartLooperState = (LooperState) EditorGUILayout.EnumPopup("State at start", clipTemplate.StartLooperState);

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.TextField("Current state", clipTemplate.RunningLooperState.ToString());
        }

        GUILayout.EndVertical();
    }
}