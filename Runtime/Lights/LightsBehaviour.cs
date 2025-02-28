using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This acts as our data for the clip to write to
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    public class LightsBehaviour : PlayableBehaviour
    {
        public float intensity;
        public float range;
        public Color32 color;
    }
}