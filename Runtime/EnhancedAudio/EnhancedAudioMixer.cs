using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    public class EnhancedAudioMixer : Mixer
    {
        private AudioSource _trackBindingAudioSource;


        protected override void ProcessingFrame()
        {
        }


        protected override void ActiveBehaviour(Behaviour genericActiveBehaviour, float easeWeight)
        {
            if (genericActiveBehaviour is not EnhancedAudioBehaviour enhancedAudioBehaviour)
            {
                return;
            }

            if (_trackBindingAudioSource == null)
            {
                _trackBindingAudioSource = (AudioSource) TrackBinding;
            }

            if (genericActiveBehaviour.ClipHasStartedOnce)
            {
                _trackBindingAudioSource.Play();
            }

            var calculatedVolume = (float) Math.Round(enhancedAudioBehaviour.Volume * easeWeight, 2);
            var clampedCalculatedVolume = Mathf.Clamp01(calculatedVolume); // Volume is always between 0 and 1
            _trackBindingAudioSource.volume = clampedCalculatedVolume; // Set the volume
            enhancedAudioBehaviour.CalculatedVolume = _trackBindingAudioSource.volume; // Display the volume in the Inspector. Just as a visual aid.    
        }
    }
}