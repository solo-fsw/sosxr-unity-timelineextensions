using System;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Enhanced Audio track. Starts playback and applies all audio settings on clip start, adjusts volume
    ///     each frame using the ease weight, and stops the <see cref="AudioSource"/> when the clip ends.
    /// </summary>
    public class EnhancedAudioMixer : Mixer
    {
        public AudioSource Binding;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = (AudioSource)TrackBinding;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not EnhancedAudioBehaviour behaviour)
            {
                return;
            }

            if (Binding == null)
            {
                Debug.LogError($"{GetType().Name}: There is nothing bound to this Track. Did you forget to set it??");

                return;
            }

            Binding.clip = behaviour.Audio;
            Binding.loop = behaviour.Loop;
            Binding.pitch = behaviour.Pitch;
            Binding.spatialBlend = behaviour.SpatialBlend;
            Binding.minDistance = behaviour.Distance.x;
            Binding.maxDistance = behaviour.Distance.y;
            Binding.rolloffMode = AudioRolloffMode.Custom;
            Binding.SetCustomCurve(AudioSourceCurveType.CustomRolloff, behaviour.VolumeOverDistance);
            Binding.Play();
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not EnhancedAudioBehaviour behaviour)
            {
                return;
            }

            if (Binding == null)
            {
                return;
            }

            float calculatedVolume = (float)Math.Round(behaviour.MaxVolume * easeWeight, 3);
            Binding.volume = Mathf.Clamp01(calculatedVolume); // Volume is always between 0 and 1
        }

        protected override void ClipEnd(Behaviour activeBehaviour) => Binding?.Stop();
    }
}
