using System;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class LightsBehaviour : Behaviour
    {
        public float Intensity;
        public Color Color;
        public float Range;

        [HideInInspector] public float OriginalIntensity;
        [HideInInspector] public Color OriginalColor;
        [HideInInspector] public float OriginalRange;
    }
}