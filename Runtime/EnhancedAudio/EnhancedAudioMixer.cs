using System;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class EnhancedAudioMixer : Mixer
    {
        public AudioSource AudioSource;


        protected override void ActiveBehaviourClipStart(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not EnhancedAudioBehaviour behaviour)
            {
                return;
            }

            Debug.Log("Clip started once, playing audio");
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


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not EnhancedAudioBehaviour behaviour)
            {
                return;
            }


            var calculatedVolume = (float) Math.Round(behaviour.MaxVolume * easeWeight, 3);
            AudioSource.volume = Mathf.Clamp01(calculatedVolume); // Volume is always between 0 and 1
        }


        protected override void ActiveBehaviourClipEnd(Behaviour activeBehaviour)
        {
            AudioSource.Stop();
        }
    }
}