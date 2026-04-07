using System;
using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data for the TextMeshPro track. Stores the text string and color to display on the bound
    ///     <see cref="TMPro.TextMeshProUGUI"/>. Alpha is driven by the clip's ease weight so easing acts as a fade.
    ///     Based on <a href="https://youtu.be/12bfRIvqLW4">GameDevGuide</a>.
    /// </summary>
    [Serializable]
    public class TextBehaviour : Behaviour
    {
        /// <summary>Text content to display on the bound <see cref="TMPro.TextMeshProUGUI"/>.</summary>
        public string Text;

        /// <summary>Base color for the text. Alpha is overwritten each frame by the clip's ease weight (0 = transparent, 1 = full).</summary>
        public Color TextColor;
    }
}
