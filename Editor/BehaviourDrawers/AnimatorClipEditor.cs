using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    [CustomEditor(typeof(AnimatorClip))]
    public class AnimatorClipEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var clip = target as AnimatorClip;

            if (clip == null)
            {
                return;
            }

            if (GUILayout.Button(nameof(clip.MatchDurationToStartClip)))
            {
                clip.MatchDurationToStartClip();
            }
        }
    }
}