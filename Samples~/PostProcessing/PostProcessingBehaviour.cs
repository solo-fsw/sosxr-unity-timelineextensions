using UnityEngine;
using UnityEngine.Rendering;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This acts as our data for the clip to write to
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    public class PostProcessingBehaviour : Behaviour
    {
        public Volume Volume;
        [Range(0f, 1f)] public float MaxWeight;
    }
}