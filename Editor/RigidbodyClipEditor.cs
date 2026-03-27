#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions.EditorScripts
{
    [CustomEditor(typeof(RigidbodyClip))]
    public class RigidbodyClipEditor : UnityEditor.Editor
    {
        private RigidbodyClip _clip;

        private void OnEnable()
        {
            _clip = (RigidbodyClip)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();

            if (_clip.AddForce)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Force Preview", EditorStyles.boldLabel);
                DrawForcePreview();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawForcePreview()
        {
            Rigidbody rb = _clip?.TrackBinding as Rigidbody;

            if (rb != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"Target: {rb.name}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    $"Mass: {rb.mass:F3} kg | Drag: {rb.linearDamping:F3}",
                    EditorStyles.miniLabel
                );
                EditorGUILayout.Space(5);

                float amount = _clip.Amount;
                ForceMode mode = _clip.ForceMode;

                switch (mode)
                {
                    case ForceMode.Force:
                        float acceleration = amount / rb.mass;
                        EditorGUILayout.LabelField("Force Mode: Force", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Applied: {amount:F1} N");
                        EditorGUILayout.LabelField($"Acceleration: {acceleration:F2} m/s²");
                        EditorGUILayout.LabelField(
                            $"Velocity after 1s: {acceleration:F2} m/s ({(acceleration * 3.6f):F1} km/h)"
                        );
                        break;

                    case ForceMode.Impulse:
                        float velocityChange = amount / rb.mass;
                        float effectiveForce = amount / Time.fixedDeltaTime;
                        EditorGUILayout.LabelField("Force Mode: Impulse", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Applied: {amount:F1} N⋅s");
                        EditorGUILayout.LabelField(
                            $"Velocity change: {velocityChange:F2} m/s ({(velocityChange * 3.6f):F1} km/h)"
                        );
                        EditorGUILayout.LabelField(
                            $"Effective force: {effectiveForce:F0} N (over 1 frame)"
                        );
                        break;

                    case ForceMode.VelocityChange:
                        EditorGUILayout.LabelField(
                            "Force Mode: VelocityChange",
                            EditorStyles.boldLabel
                        );
                        EditorGUILayout.LabelField($"Applied: {amount:F2} m/s velocity change");
                        EditorGUILayout.LabelField(
                            "Note: Mass independent - same result for any object"
                        );
                        EditorGUILayout.LabelField(
                            $"Equivalent impulse: {amount * rb.mass:F2} N⋅s"
                        );
                        break;

                    case ForceMode.Acceleration:
                        EditorGUILayout.LabelField(
                            "Force Mode: Acceleration",
                            EditorStyles.boldLabel
                        );
                        EditorGUILayout.LabelField($"Applied: {amount:F2} m/s²");
                        EditorGUILayout.LabelField("Note: Mass independent");
                        EditorGUILayout.LabelField($"Velocity after 1s: {amount:F2} m/s");
                        break;
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "No Rigidbody bound to track. Bind a Rigidbody to see force calculations.",
                    MessageType.Info
                );
            }
        }

        private Rigidbody GetBoundRigidbody()
        {
            return _clip.TrackBinding as Rigidbody;
        }
    }
}
#endif
