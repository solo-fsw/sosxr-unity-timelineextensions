using System;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(TimelineSpeedBehaviour))]
public class TimelineSpeedDrawer : PropertyDrawer
{
    private SerializedProperty _exposedReference;
    private const string WarningMessage = "Be careful when setting Timeline speed to anything else than 1." +
                                          "\nThis ONLY sets the speed of this Timeline." +
                                          "\n\nNot all systems respond to this setting." +
                                          "\nSystems like the AnimatorController, Audio, and others, are unaffected." +
                                          "\n\nUse TimeScale track if you don't want to have these issues.";


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var clip = property.serializedObject.targetObject as TimelineSpeedClip;

        if (!clip)
        {
            return;
        }

        var clipTemplate = clip.template;

        if (clipTemplate == null)
        {
            return;
        }

        _exposedReference ??= property.FindPropertyRelative(nameof(clipTemplate.resumerReference));

        DrawStartSpeedVariables(clipTemplate);

        if (clipTemplate.setSpeedAtStart && clipTemplate.speedAtStart == 0)
        {
            DrawTimelineController(clipTemplate);
        }

        DrawEndSpeedVariables(clipTemplate);

        if (clipTemplate.setSpeedAtEnd && clipTemplate.speedAtEnd == 0)
        {
            DrawTimelineController(clipTemplate);
        }

        DrawWarningMessage(clipTemplate);
    }


    private static void DrawStartSpeedVariables(TimelineSpeedBehaviour clipTemplate)
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        clipTemplate.setSpeedAtStart = EditorGUILayout.ToggleLeft("Set speed at clip start", clipTemplate.setSpeedAtStart);

        if (clipTemplate.setSpeedAtStart)
        {
            clipTemplate.speedAtStart = EditorGUILayout.Slider("Speed at start", (float) clipTemplate.speedAtStart, 0f, 1f);
        }

        GUILayout.EndVertical();
    }


    private void DrawTimelineController(TimelineSpeedBehaviour clipTemplate)
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);

        clipTemplate.handControlTo = EditorGUILayout.ToggleLeft("Hand control to other class", clipTemplate.handControlTo);

        if (clipTemplate.handControlTo)
        {
            EditorGUILayout.PropertyField(_exposedReference, new GUIContent("Resumer"));
        }

        GUILayout.EndVertical();
    }


    private static void DrawEndSpeedVariables(TimelineSpeedBehaviour clipTemplate)
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        clipTemplate.setSpeedAtEnd = EditorGUILayout.ToggleLeft("Set speed clip end", clipTemplate.setSpeedAtEnd);

        if (clipTemplate.setSpeedAtEnd)
        {
            clipTemplate.speedAtEnd = EditorGUILayout.Slider("Speed at end", (float) clipTemplate.speedAtEnd, 0f, 1f);
        }

        GUILayout.EndVertical();
    }


    private static void DrawWarningMessage(TimelineSpeedBehaviour clipTemplate)
    {
        if ((!clipTemplate.setSpeedAtStart || !(Math.Abs(clipTemplate.speedAtStart - 1) > 0.01f)) && (!clipTemplate.setSpeedAtEnd || !(Math.Abs(clipTemplate.speedAtEnd - 1) > 0.01f)))
        {
            return;
        }

        EditorGUILayout.Space(15f);
        EditorGUILayout.HelpBox(WarningMessage, MessageType.Warning, true);
        EditorGUILayout.Space(15f);
    }
}