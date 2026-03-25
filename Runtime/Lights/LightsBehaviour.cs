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
        public Color Color = new Color(255f, 244f, 214f);

        /// <summary>Target range to lerp towards. Blended against <see cref="OriginalRange"/> using ease weight.</summary>
        public float Range = 10;

        [HideInInspector] public float OriginalIntensity;
        [HideInInspector] public Color OriginalColor;
        [HideInInspector] public float OriginalRange;
    }
}
