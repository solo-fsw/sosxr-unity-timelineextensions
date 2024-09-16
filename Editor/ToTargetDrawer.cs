using System;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(ToTargetBehaviour))]
public class ToTargetDrawer : PropertyDrawer
{
    private const int roundDecimal = 2;
    private const string meters = " meters";
    private const string seconds = " sec";


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var clip = property.serializedObject.targetObject as ToTargetClip;

        if (!clip)
        {
            return;
        }

        var clipTemplate = clip.template;

        if (clipTemplate.trackBinding == null)
        {
            return;
        }

        if (clip.StartingPoint == null || clip.Target == null)
        {
            return;
        }

        CreateChangeableValuesBox(clipTemplate);

        GUILayout.Space(5f);

        CreateForceClipLengthToggle(clipTemplate);

        GUILayout.Space(5f);

        if (clip.Behaviour == null)
        {
            return;
        }

        CreateDisplayValuesBox(clip);
    }


    private static void CreateChangeableValuesBox(ToTargetBehaviour clipTemplate)
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        clipTemplate.axisToUse = EditorGUILayout.Vector3IntField("Axis to Use", clipTemplate.axisToUse);
        clipTemplate.rotateSpeed = EditorGUILayout.Slider("Rotate Speed", clipTemplate.rotateSpeed, 0f, 4f);
        clipTemplate.moveSpeed = EditorGUILayout.Slider("Move Speed", clipTemplate.moveSpeed, 0f, 4f);
        clipTemplate.stoppingDistance = EditorGUILayout.Slider("Stopping Distance", clipTemplate.stoppingDistance, 0f, 5f);
        GUILayout.EndVertical();
    }


    private static void CreateForceClipLengthToggle(ToTargetBehaviour clipTemplate)
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        clipTemplate.forceClipLength = EditorGUILayout.ToggleLeft("Force Clip Length", clipTemplate.forceClipLength);
        GUILayout.EndVertical();
    }


    private static void CreateDisplayValuesBox(ToTargetClip clip)
    {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);

        using (new EditorGUI.DisabledScope(true))
        {
            ShowLabels();

            ShowValuesForLabels(clip);
        }

        GUILayout.EndHorizontal();
    }


    private static void ShowLabels()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        CreateBoldLabelWithSpaceAbove(0, "STARTING DISTANCE");
        EditorGUILayout.LabelField("Start distance");
        EditorGUILayout.LabelField("Start distance minus stop distance");

        CreateBoldLabelWithSpaceAbove(10, "DURATION");
        EditorGUILayout.LabelField("Duration to target");

        CreateBoldLabelWithSpaceAbove(10, "REMAINING");
        EditorGUILayout.LabelField("Remaining distance");
        EditorGUILayout.LabelField("Remaining distance minus stop distance");

        EditorGUILayout.EndVertical();
    }


    private static void CreateBoldLabelWithSpaceAbove(int space, string labelText)
    {
        GUILayout.Space(space);
        EditorGUILayout.LabelField(labelText, EditorStyles.boldLabel);
    }


    private static void ShowValuesForLabels(ToTargetClip clip)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        CreateBoldLabelWithSpaceAbove(0, ""); // Creates empty space to match other side
        EditorGUILayout.LabelField(Math.Round(clip.Behaviour.startingDistance, roundDecimal) + meters);
        EditorGUILayout.LabelField(Math.Round(clip.Behaviour.startMinStopDistance, roundDecimal) + meters);

        CreateBoldLabelWithSpaceAbove(10, "");
        EditorGUILayout.LabelField(Math.Round(clip.Behaviour.durationToTarget, roundDecimal) + seconds);

        CreateBoldLabelWithSpaceAbove(10, "");
        EditorGUILayout.LabelField(Math.Round(clip.Behaviour.remainingDistance, roundDecimal) + meters);
        EditorGUILayout.LabelField(Math.Round(clip.Behaviour.remainingMinStopDistance, roundDecimal) + meters);

        EditorGUILayout.EndVertical();
    }
}