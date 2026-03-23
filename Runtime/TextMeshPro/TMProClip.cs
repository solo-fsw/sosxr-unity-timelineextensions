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
    public class TMProClip : Clip
    {
        [field: SerializeField] public string Text { get; }
        [Tooltip("Do not use Alpha, because alpha is used for easing")]
        [field: SerializeField] public Color TextColor { get; }

        public override ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<TMProBehaviour> playable = ScriptPlayable<TMProBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();

    behaviour.Text = Text;
            behaviour.TextColor = TextColor;
            behaviour.InitializeBehaviour(TimelineClip, TrackBinding);

            return playable;
    }
    }
}




