using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SOSXR.TimelineExtensions.EditorScripts
{
    /// <summary>
    ///     Custom property drawer for <see cref="AnimatorBehaviour"/>. Renders the state name as a dropdown from the bound
    ///     Animator Controller, shows a HelpBox warning when a non-looping animation is shorter than the clip, and provides
    ///     a "Match Duration to Animation" button.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimatorBehaviour))]
    public class AnimatorBehaviourDrawer : PropertyDrawer
    {
        /// <summary>
        ///     Draws the state dropdown for the bound <see cref="AnimatorClip"/>, a warning when a non-looping animation is shorter
        ///     than the Timeline clip, and the "Match Duration to Animation" button.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property?.serializedObject?.targetObject == null)
            {
                return;
            }

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
                var options = new List<string>(clip.StateNames);
                int index = options.IndexOf(clip.Template.StateName);

                // A name the controller does not have (renamed, or not set yet) is shown, never silently replaced.
                if (index < 0)
                {
                    string current = clip.Template.StateName;
                    options.Insert(0, string.IsNullOrEmpty(current) ? "<none>" : $"<missing: {current}>");
                    index = 0;
                }

                int offset = options.Count - clip.StateNames.Count;

                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(
                    new Rect(position.x, currentY, position.width, lineHeight),
                    "State", index, options.ToArray());

                if (EditorGUI.EndChangeCheck() && index >= offset)
                {
                    Undo.RecordObject(clip, "Change Animator State");
                    clip.Template.StateName = clip.StateNames[index - offset];
                    EditorUtility.SetDirty(clip);
                }

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

        /// <summary>
        ///     Returns the height the drawer needs. Adds space for the loop warning when that warning is shown.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property?.serializedObject?.targetObject == null)
            {
                return EditorGUIUtility.singleLineHeight + 2f;
            }

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
