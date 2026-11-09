using System;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Enhanced Audio track. Starts playback and applies all audio settings on clip start, adjusts volume
    ///     each frame using the ease weight, and stops the <see cref="AudioSource"/> when the clip ends.
    ///     All clips share the bound AudioSource: the clip that started last owns it. An older clip that ends while a newer
    ///     one is playing does not stop or change the source.
    /// </summary>
    public class EnhancedAudioMixer : Mixer
    {
        public AudioSource Binding;

        private EnhancedAudioBehaviour _currentBehaviour;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = TrackBinding as AudioSource;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not EnhancedAudioBehaviour behaviour)
            {
                return;
            }

            if (Binding == null)
            {
                Debug.LogError($"{TypeName}: There is nothing bound to this Track. Did you forget to set it??");

                return;
            }

            _currentBehaviour = behaviour;

            Binding.clip = behaviour.Audio;
            Binding.loop = behaviour.Loop;
            Binding.pitch = behaviour.Pitch;
            Binding.spatialBlend = behaviour.SpatialBlend;
            Binding.minDistance = behaviour.Distance.x;
            Binding.maxDistance = behaviour.Distance.y;
            Binding.rolloffMode = AudioRolloffMode.Custom;
            Binding.SetCustomCurve(AudioSourceCurveType.CustomRolloff, behaviour.VolumeOverDistance);

            if (behaviour.Audio == null)
            {
                return;
            }

            var startTime = ComputeAudioStartTime(behaviour.ActivationLocalTime, behaviour.Audio.length, behaviour.Loop);

            if (startTime < 0)
            {
                Binding.Stop(); // Started past the end of a non-looping clip: nothing left to hear.

                return;
            }

            Binding.time = startTime;
            Binding.Play();
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not EnhancedAudioBehaviour behaviour)
            {
                return;
            }

            if (Binding == null || behaviour != _currentBehaviour)
            {
                return;
            }

            var calculatedVolume = (float)Math.Round(behaviour.MaxVolume * easeWeight, 3);
            Binding.volume = Mathf.Clamp01(calculatedVolume); // Volume is always between 0 and 1
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (!ShouldStopOnEnd(activeBehaviour, _currentBehaviour))
            {
                return;
            }

            _currentBehaviour = null;

            if (Binding != null)
            {
                Binding.Stop();
            }
        }

        /// <summary>True when the ending clip still owns the AudioSource. False when a newer clip took it over.</summary>
        public static bool ShouldStopOnEnd(Behaviour endingBehaviour, Behaviour currentBehaviour)
        {
            return endingBehaviour != null && endingBehaviour == currentBehaviour;
        }

        /// <summary>
        ///     Where in the AudioClip playback must start, given the clip-local time at which the Timeline clip became active.
        ///     Lets playback start in sync after a seek or a mid-clip start.
        /// </summary>
        /// <returns>Seconds into the audio, or -1 when a non-looping clip started past the end of its audio.</returns>
        public static float ComputeAudioStartTime(double localTime, float audioLength, bool loop)
        {
            if (audioLength <= 0)
            {
                return -1f;
            }

            if (localTime <= 0)
            {
                return 0f;
            }

            if (loop)
            {
                return (float)(localTime % audioLength);
            }

            if (localTime >= audioLength)
            {
                return -1f;
            }

            return (float)localTime;
        }
    }
}
