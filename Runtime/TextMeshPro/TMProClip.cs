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
        public string text; // Allows us to set the text in the editor.

        [Tooltip("Do not use Alpha, because alpha is used for easing")]
        public Color color; // Allows us to set the color in the editor.


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) // Here we write our logic for creating the playable behaviour
        {
            var playable = ScriptPlayable<TMProBehaviour>.Create(graph); // Create a playable, using the constructor

            var behaviour = playable.GetBehaviour(); // Get behaviour

            behaviour.text = text; // Then, set the text on the behaviour, from the text on clip
            behaviour.color = color;

            return playable;
        }
    }
}