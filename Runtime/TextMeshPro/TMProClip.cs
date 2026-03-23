using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the TextMeshPro track. Set the text and base color per clip in the Inspector.
    ///     Do not adjust the Alpha channel — it is automatically driven by the ease weight for fading.
    ///     Based on <a href="https://youtu.be/12bfRIvqLW4">GameDevGuide</a>.
    /// </summary>
    public class TMProClip : PlayableAsset
    {
        [field: SerializeField] public string Text { get; }
        [Tooltip("Do not use Alpha, because alpha is used for easing")]
        [field: SerializeField] public Color TextColor { get; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) // Here we write our logic for creating the playable behaviour
        {
            ScriptPlayable<TMProBehaviour> playable = ScriptPlayable<TMProBehaviour>.Create(graph); // Create a playable, using the constructor

            var behaviour = playable.GetBehaviour(); // Get behaviour

            behaviour.Text = Text; // Then, set the text on the behaviour, from the text on clip
            behaviour.TextColor = TextColor;

            return playable;
        }
    }
}
