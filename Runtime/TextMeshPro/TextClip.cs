using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the TextMeshPro track. Set the text and base color per clip in the Inspector.
    ///     Do not adjust the Alpha channel — it is automatically driven by the ease weight for fading.
    ///     Based on <a href="https://youtu.be/12bfRIvqLW4">GameDevGuide</a>.
    /// </summary>
    public class TextClip : Clip
    {
        [SerializeField]
        private string m_text;
        public string Text
        {
            get { return m_text; }
        }

        [SerializeField]
        [Tooltip("This does not use alpha, because alpha is used for easing")]
        [ColorUsage(false)]
        private Color m_textColor;
        public Color TextColor
        {
            get { return m_textColor; }
        }

        public override ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<TextBehaviour> playable = ScriptPlayable<TextBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();

            behaviour.Text = m_text;
            behaviour.TextColor = m_textColor;
            behaviour.InitializeBehaviour(TimelineClip, TrackBinding);

            return playable;
        }
    }
}
