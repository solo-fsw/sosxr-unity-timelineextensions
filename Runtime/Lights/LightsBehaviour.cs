using System;
using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data for the Lights track. Holds target intensity, color, and range values as well as the original
    ///     values captured at clip creation so the mixer can lerp back to them on blend-out.
    /// </summary>
    [Serializable]
    public class LightsBehaviour : Behaviour
    {
        /// <summary>Target intensity to lerp towards. Blended against <see cref="OriginalIntensity"/> using ease weight.</summary>
        public float Intensity = 1;

        /// <summary>Target color to lerp towards. Blended against <see cref="OriginalColor"/> using ease weight.</summary>
        public Color Color = new Color(1f, 244f / 255f, 214f / 255f); // Warm white

        /// <summary>Target range to lerp towards. Blended against <see cref="OriginalRange"/> using ease weight.</summary>
        public float Range = 10;

        [HideInInspector] public float OriginalIntensity;
        [HideInInspector] public Color OriginalColor;
        [HideInInspector] public float OriginalRange;

        /// <summary>
        ///     Light colours are 0-1. Older versions defaulted to a 0-255 colour (about 255x overbright).
        ///     Returns the colour divided by 255 when any RGB channel is above 1; otherwise returns it unchanged.
        /// </summary>
        public static Color NormalizeLegacyColor(Color color)
        {
            if (color.r <= 1f && color.g <= 1f && color.b <= 1f)
            {
                return color;
            }

            return new Color(color.r / 255f, color.g / 255f, color.b / 255f, Mathf.Clamp01(color.a));
        }
    }
}
