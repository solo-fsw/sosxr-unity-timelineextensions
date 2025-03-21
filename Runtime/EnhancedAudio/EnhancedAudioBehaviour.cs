using System;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class EnhancedAudioBehaviour : Behaviour
    {
        [Tooltip("Volume is multiplied by the ease weight.")]
        [Range(0f, 1f)] public float MaxVolume = 1f;
        [Range(-3f, 3f)] public float Pitch = 1f;
        [Tooltip("0 is 2D, 1 is 3D")]
        [Range(0f, 1f)] public float SpatialBlend = 1f;
        public Vector2 Distance = new(0.25f, 10f);
        public AnimationCurve VolumeOverDistance = AnimationCurve.EaseInOut(0, 1, 1, 0.1f);

        [HideInInspector] public AudioClip Audio;
        [HideInInspector] public bool Loop = false;
    }
}