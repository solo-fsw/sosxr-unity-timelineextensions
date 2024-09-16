using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof (AnimatorBehaviour))]
public class AnimatorDrawer : PropertyDrawer
{
	private readonly List<string> parameterOptions = new List<string>();
	private bool listIsPopulated = false;

	private const string selectParameter = "select Parameter...";

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var clip = property.serializedObject.targetObject as AnimatorClip;

		if (!clip)
		{
			return;
		}

		var clipTemplate = clip.template;
		if (clipTemplate == null)
		{
			return;
		}

		if (clipTemplate.trackBinding == null)
		{
			return;
		}

		if (clip as BoolClip)
		{
			CreateBoolClipInspector(clipTemplate);
		}
		else if (clip as TriggerClip)
		{
			CreateTriggerClipInspector(clipTemplate);
		}
	}


	private void CreateBoolClipInspector(AnimatorBehaviour clipTemplate)
	{
		var isSelected = clipTemplate.boolName != "";

		AddParametersToList(clipTemplate, isSelected, AnimatorControllerParameterType.Bool);

		var optionIndex = SelectedOptionIndex(clipTemplate.boolName, isSelected);

		EditorGUI.BeginChangeCheck();
		var selection = EditorGUILayout.Popup(optionIndex, parameterOptions.ToArray());

		if (EditorGUI.EndChangeCheck())
		{
			clipTemplate.boolName = parameterOptions[selection];
			clipTemplate.boolIndex = Animator.StringToHash(parameterOptions[selection]);
		}

		if (clipTemplate.boolName != "")
		{
			clipTemplate.boolValue = EditorGUILayout.Toggle("'" + clipTemplate.boolName + "' value", clipTemplate.boolValue);
			clipTemplate.resetBool = EditorGUILayout.Toggle("Reset '" + clipTemplate.boolName + "' to original value", clipTemplate.resetBool);
		}
	}


	private void CreateTriggerClipInspector(AnimatorBehaviour clipTemplate)
	{
		var isSelected = clipTemplate.triggerName != "";

		AddParametersToList(clipTemplate, isSelected, AnimatorControllerParameterType.Trigger);

		var optionIndex = SelectedOptionIndex(clipTemplate.triggerName, isSelected);

		EditorGUI.BeginChangeCheck();
		var selection = EditorGUILayout.Popup(optionIndex, parameterOptions.ToArray());

		if (EditorGUI.EndChangeCheck())
		{
			clipTemplate.triggerName = parameterOptions[selection];
			clipTemplate.triggerIndex = Animator.StringToHash(parameterOptions[selection]);
		}

		if (clipTemplate.triggerName != "")
		{
			clipTemplate.triggerOnce = EditorGUILayout.Toggle("Trigger '" + clipTemplate.triggerName + "' only once", clipTemplate.triggerOnce);
			clipTemplate.forceTriggerClipLength = EditorGUILayout.Toggle("Force '" + clipTemplate.triggerName + "' clip duration", clipTemplate.forceTriggerClipLength);
		}

		GUILayout.Space(15f);
	}




	private int SelectedOptionIndex(string parameterName, bool isSelected)
	{
		var optionIndex = 0;

		if (isSelected)
		{
			parameterOptions.Remove(selectParameter);
			optionIndex = parameterOptions.FindIndex(n => n.Contains(parameterName));
		}

		return optionIndex;
	}


	private void AddParametersToList(AnimatorBehaviour clipTemplate, bool isSelected, AnimatorControllerParameterType parameterType)
	{
		if (!listIsPopulated)
		{
			if (!isSelected)
			{
				parameterOptions.Add(selectParameter);
			}

			var anim = clipTemplate.trackBinding;

			for (int i = 0; i < anim.parameterCount; i++)
			{
				if (anim.GetParameter(i).type == parameterType)
				{
					parameterOptions.Add(anim.GetParameter(i).name);
				}
			}

			listIsPopulated = true;
		}
	}



}
