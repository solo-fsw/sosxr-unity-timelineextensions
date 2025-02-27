using System;
using UnityEditor;
using UnityEngine;


/// <summary>
///     Based off of Unity Timeline sample pack : Time Dilation
///     From: https://docs.unity3d.com/Packages/com.unity.timeline@1.6/manual/smpl_about.html
/// </summary>
[CustomPropertyDrawer(typeof(TimeScaleBehaviour))]
public class TimeScaleDrawer : PropertyDrawer
{
    private const string WarningMessage = "Be careful when setting the game Time Scale." +
                                          "\nThis effects ALL systems, and may have an effect on the HMD position and rotation calculations.";


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var clip = property.serializedObject.targetObject as TimeScaleClip;

        if (!clip)
        {
            return;
        }

        var clipTemplate = clip.template;

        clipTemplate.timeScale = EditorGUILayout.Slider("Time Scale", clipTemplate.timeScale, -5f, 6f);

        if (Math.Abs(clipTemplate.timeScale - 1) > 0.01f)
        {
            EditorGUILayout.Space(15f);
            EditorGUILayout.HelpBox(WarningMessage, MessageType.Warning, true);
        }
    }
}