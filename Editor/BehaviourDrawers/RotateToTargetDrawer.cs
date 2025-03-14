using System.Text;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions.Editor
{
    [CustomPropertyDrawer(typeof(RotateToTargetBehaviour))]
    public class RotateToTargetDrawer : PropertyDrawer
    {
        private SerializedProperty _exposedReference;
        private const string ClipDisplayname = "m_Clip.m_DisplayName";
        private const string ItemDisplayname = "m_Item.m_DisplayName";


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Debug.Log("RotateToTargetDrawer");
            var clip = property.serializedObject.targetObject as RotateToTargetClip;

            if (!clip)
            {
                return;
            }

            var clipTemplate = clip.Template;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var director = TimelineEditor.inspectedDirector;
            _exposedReference ??= property.FindPropertyRelative(nameof(clipTemplate.TargetRef));
            EditorGUILayout.PropertyField(_exposedReference, new GUIContent("Target Ref"));
            clipTemplate.Target = clipTemplate.TargetRef.Resolve(director);
            clipTemplate.AxisToUse = EditorGUILayout.Vector3IntField(new GUIContent("Axis to use", "Which axis to use for calculations? 0 = don't use, 1 = use"), clipTemplate.AxisToUse);
            clipTemplate.RotateSpeed = EditorGUILayout.FloatField("Rotation Speed", clipTemplate.RotateSpeed);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginDisabledGroup(true);
            const int roundedTo = 2;
            EditorGUILayout.LabelField(new GUIContent(new StringBuilder("Rounded to " + roundedTo + " decimals").ToString()), new GUIStyle {alignment = TextAnchor.UpperRight});
            EditorGUILayout.Vector3Field(new GUIContent("Displacement from Target", "The displacement"), clipTemplate.DisplacementFromTarget);
            EditorGUILayout.Vector3Field(new GUIContent("Direction to Target", "The direction to target"), clipTemplate.DirectionToTarget);
            EditorGUILayout.FloatField(new GUIContent("Distance to Target", "Distance to Target"), clipTemplate.DistanceToTarget);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            // Assume that the currently selected object is the internal class UnityEditor.Timeline.EditorClip
            // this gives you access to the clip start, duration etc.
            var editorGUI = new SerializedObject(Selection.activeObject);

            SetTitle(editorGUI, clipTemplate);

            editorGUI.ApplyModifiedProperties();
        }


        private static void SetTitle(SerializedObject editorGUI, PlayableBehaviour template)
        {
            var clipTemplate = (RotateToTargetBehaviour) template;

            var title = editorGUI.FindProperty(ClipDisplayname) ?? editorGUI.FindProperty(ItemDisplayname);

            if (title == null)
            {
                return;
            }

            if (clipTemplate.Target != null)
            {
                title.stringValue = new StringBuilder("Rotate to: " + clipTemplate.Target.name).ToString();
            }
        }
    }
}