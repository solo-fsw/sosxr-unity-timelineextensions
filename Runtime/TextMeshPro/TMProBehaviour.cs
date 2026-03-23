using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data for the TextMeshPro track. Stores the text string and color to display on the bound
    ///     <see cref="TMPro.TextMeshProUGUI"/>. Alpha is driven by the clip's ease weight so easing acts as a fade.
    ///     Based on <a href="https://youtu.be/12bfRIvqLW4">GameDevGuide</a>.
    /// </summary>
    public class TMProBehaviour : PlayableBehaviour
    {
        public string Text; // Act as our data for the clip to write to
        public Color TextColor;
    }
}
