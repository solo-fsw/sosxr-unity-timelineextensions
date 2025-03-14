using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceBehaviour))]
    public class InterfaceBehaviourDrawer : PropertyDrawer
    {
        private SerializedProperty exposedReference;

        private GameObject _interactObject;
        private ITimeControl _iTimeControl;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var clip = property.serializedObject.targetObject as InterfaceClip;

            if (!clip)
            {
                return;
            }

            exposedReference ??= property.FindPropertyRelative(nameof(InterfaceBehaviour.InterfaceObjectReference));

            DrawLoopBreaker();

            DrawHelpBox(clip);
        }


        private void DrawLoopBreaker()
        {
            if (exposedReference == null)
            {
                return;
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(exposedReference, new GUIContent("Interface Object"));
            GUILayout.EndVertical();
        }


        private static void DrawHelpBox(InterfaceClip clip)
        {
            if (clip.Template.Interface != null)
            {
                return;
            }

            EditorGUILayout.HelpBox("No GameObject bound which has an ITimeControl interface", MessageType.Warning);
        }
    }
}