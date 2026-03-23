using UnityEditor;
using UnityEngine;

namespace SOSXR.TimelineExtensions.EditorScripts
{
    [CustomPropertyDrawer(typeof(AnimatorBehaviour))]
    public class AnimatorBehaviourDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            AnimatorClip clip = property.serializedObject.targetObject as AnimatorClip;

            if (clip == null || clip.Template == null)
            {
                return;
            }

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 2f;
            float currentY = position.y;

            if (clip.StateNames != null && clip.StateNames.Count > 0)
            {
                int index = Mathf.Max(0, clip.StateNames.IndexOf(clip.Template.StateName));
                index = EditorGUI.Popup(
                    new Rect(position.x, currentY, position.width, lineHeight),
                    "State", index, clip.StateNames.ToArray());
                clip.Template.StateName = clip.StateNames[index];
                currentY += lineHeight + spacing;
            }

            var animator = GetAnimatorFromClip(clip);
            if (animator != null && !string.IsNullOrEmpty(clip.Template.StateName))
            {
                ShowLoopWarning(clip, animator, ref currentY, position);
            }

            if (GUI.Button(
                new Rect(position.x, currentY, position.width, lineHeight),
                "Match Duration to Animation"))
            {
                MatchClipDuration(clip, animator);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            AnimatorClip clip = property.serializedObject.targetObject as AnimatorClip;
            float height = EditorGUIUtility.singleLineHeight + 2f;

            if (clip?.Template != null && !string.IsNullOrEmpty(clip.Template.StateName))
            {
                var animator = GetAnimatorFromClip(clip);
                if (animator != null)
                {
                    var animClip = animator.GetStateAnimationClip(clip.Template.StateName);
                    if (animClip != null && !animClip.isLooping && clip.TimelineClip != null)
                    {
                        float animLength = animClip.length;
                        float timelineDuration = (float)clip.TimelineClip.duration;
                        if (timelineDuration > animLength * 1.01f)
                        {
                            height += EditorGUIUtility.singleLineHeight * 2 + 4f;
                        }
                    }
                }
                height += EditorGUIUtility.singleLineHeight + 2f;
            }

            return height;
        }

        private Animator GetAnimatorFromClip(AnimatorClip clip)
        {
            if (clip.TimelineClip == null)
            {
                return null;
            }

            var track = clip.TimelineClip.GetParentTrack();
            if (track == null)
            {
                return null;
            }

            var director = UnityEditor.Timeline.TimelineEditor.inspectedDirector;
            if (director == null)
            {
                return null;
            }

            return director.GetGenericBinding(track) as Animator;
        }

        private void ShowLoopWarning(AnimatorClip clip, Animator animator, ref float currentY, Rect position)
        {
            var animClip = animator.GetStateAnimationClip(clip.Template.StateName);
            if (animClip == null || clip.TimelineClip == null)
            {
                return;
            }

            if (animClip.isLooping)
            {
                return;
            }

            float animLength = animClip.length;
            float timelineDuration = (float)clip.TimelineClip.duration;

            if (timelineDuration > animLength * 1.01f)
            {
                float warningHeight = EditorGUIUtility.singleLineHeight * 2;
                EditorGUI.HelpBox(
                    new Rect(position.x, currentY, position.width, warningHeight),
                    $"Animation is non-looping ({animLength:F2}s) but Timeline clip is longer ({timelineDuration:F2}s). " +
                    $"The animation will freeze at the end.",
                    MessageType.Warning);
                currentY += warningHeight + 2f;
            }
        }

        private void MatchClipDuration(AnimatorClip clip, Animator animator)
        {
            if (animator == null || clip?.TimelineClip == null)
            {
                return;
            }

            var track = clip.TimelineClip.GetParentTrack();
            if (track == null)
            {
                return;
            }

            string stateName = clip.Template?.StateName;
            if (string.IsNullOrEmpty(stateName))
            {
                return;
            }

            var animClip = animator.GetStateAnimationClip(stateName);
            if (animClip == null)
            {
                float duration = animator.GetStateDuration(stateName);
                if (duration > 0)
                {
                    Undo.RecordObject(track, "Match Clip Duration");
                    clip.TimelineClip.duration = duration;
                    EditorUtility.SetDirty(track);
                }
                return;
            }

            Undo.RecordObject(track, "Match Clip Duration");
            clip.TimelineClip.duration = animClip.length;
            EditorUtility.SetDirty(track);
        }
    }
}
