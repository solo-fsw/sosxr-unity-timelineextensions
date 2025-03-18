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
    public class EnhancedAudioBehaviour : PlayableBehaviour
    {
        public AudioSource TrackBinding { get; set; }

        private EnhancedAudioClip _enhancedAudioClip { get; set; }


        public void Initialize(EnhancedAudioClip enhancedAudioClip)
        {
            _enhancedAudioClip = enhancedAudioClip;


            if (TrackBinding == null || _enhancedAudioClip == null)
            {
                return;
            }

            ApplyProperties();
        }


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (TrackBinding == null || _enhancedAudioClip == null)
            {
                return;
            }


            ApplyProperties();
            TrackBinding.Play();
        }


        private void ApplyProperties()
        {
            TrackBinding.clip = _enhancedAudioClip.Clip;
            TrackBinding.volume = _enhancedAudioClip.Volume;
            TrackBinding.pitch = _enhancedAudioClip.Pitch;
            TrackBinding.mute = _enhancedAudioClip.Mute;
            TrackBinding.playOnAwake = false;
            // TrackBinding.loop = false;
            TrackBinding.spatialBlend = _enhancedAudioClip.SpatialBlend;
            TrackBinding.minDistance = _enhancedAudioClip.Distance.x;
            TrackBinding.maxDistance = _enhancedAudioClip.Distance.y;
            TrackBinding.rolloffMode = AudioRolloffMode.Linear;
            TrackBinding.SetCustomCurve(AudioSourceCurveType.CustomRolloff, _enhancedAudioClip.VolumeOverDistance);

            Debug.Log("Applying properties");
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var audioSource = playerData as AudioSource;

            if (audioSource == null)
            {
                return;
            }

            TrackBinding ??= audioSource;

            CalculateVolumeWithEase(info.weight);
        }


        private void CalculateVolumeWithEase(float clipEaseWeight)
        {
            var calculatedVolume = (float) Math.Round(clipEaseWeight * _enhancedAudioClip.Volume, 2); // Rounding because otherwise it's crazy. 
            var clampedCalculatedVolume = Mathf.Clamp01(calculatedVolume); // Volume is always between 0 and 1
            TrackBinding.volume = clampedCalculatedVolume; // Set the volume
            _enhancedAudioClip.CalculatedVolume = TrackBinding.volume; // Display the volume in the Inspector. Just as a visual aid. 
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (TrackBinding == null || _enhancedAudioClip == null)
            {
                return;
            }

            if (!Application.isPlaying)
            {
                return;
            }

            TrackBinding.Pause();
        }
    }
}