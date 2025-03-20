using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Acts as our data for the clip to write to.
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [Serializable]
    public class EnhancedAudioBehaviour : Behaviour
    {
        public AudioClip Audio;
        [HideInInspector] public AudioClip PreviousAudio;
        [Range(0f, 1f)] public float Volume = 1f;
        [Tooltip("Don't try to set this")] [Range(0f, 1f)] public float CalculatedVolume;
        [Range(-3f, 3f)] public float Pitch = 1f;
        public bool Mute = false;
        [Tooltip("0 is 2D, 1 is 3D")]
        [Range(0f, 1f)] public float SpatialBlend = 1f;

        public Vector2 Distance = new(0.25f, 10f);
        public AnimationCurve VolumeOverDistance = AnimationCurve.EaseInOut(0, 1, 1, 0.1f);
        
    }
}