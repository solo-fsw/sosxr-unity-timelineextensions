using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class EnhancedAudioMixer : Mixer
    {
        public AudioSource AudioSource;


        protected override void InitializeMixer(Playable playable)
        {
            AudioSource ??= (AudioSource) TrackBinding;
        }


        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            var behaviour = activeBehaviour as EnhancedAudioBehaviour;

            AudioSource.clip = behaviour.Audio;

            AudioSource.loop = behaviour.Loop;
            AudioSource.pitch = behaviour.Pitch;
            AudioSource.spatialBlend = behaviour.SpatialBlend;
            AudioSource.minDistance = behaviour.Distance.x;
            AudioSource.maxDistance = behaviour.Distance.y;
            AudioSource.rolloffMode = AudioRolloffMode.Custom;
            AudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, behaviour.VolumeOverDistance);

            AudioSource.Play();
        }


        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            var behaviour = activeBehaviour as EnhancedAudioBehaviour;

            var calculatedVolume = (float) Math.Round(behaviour.MaxVolume * easeWeight, 3);
            AudioSource.volume = Mathf.Clamp01(calculatedVolume); // Volume is always between 0 and 1
        }


        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            AudioSource.Stop();
        }
    }
}