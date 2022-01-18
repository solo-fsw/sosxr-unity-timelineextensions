using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof (LooperBehaviour))]
public class LooperDrawer : PropertyDrawer
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

		exposedReference ??= property.FindPropertyRelative(nameof (clipTemplate.loopBreakerReference));

		DrawLooperVariables(clipTemplate);

		DrawLoopBreaker(clipTemplate);
	}


	private static void DrawLooperVariables(LooperBehaviour clipTemplate)
	{
		GUILayout.BeginVertical(EditorStyles.helpBox);
		clipTemplate.startLooperState = (LooperBehaviour.LooperState) EditorGUILayout.EnumPopup("Looper state at start", clipTemplate.startLooperState);
		using (new EditorGUI.DisabledScope(true))
		{
			EditorGUILayout.TextField("Current looper state", clipTemplate.runningLooperState.ToString());
		}
		GUILayout.EndVertical();
	}


	private void DrawLoopBreaker(LooperBehaviour clipTemplate)
	{
		GUILayout.BeginVertical(EditorStyles.helpBox);

		clipTemplate.handControlTo = EditorGUILayout.ToggleLeft("Hand control to other class", clipTemplate.handControlTo);

		if (clipTemplate.handControlTo == true)
		{
			EditorGUILayout.PropertyField(exposedReference, new GUIContent("Looper Breaker"));
		}

		GUILayout.EndVertical();
	}
}
