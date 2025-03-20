using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(.506f, 0.435f, 0f)]
    [TrackBindingType(typeof(AudioSource))]
    [TrackClipType(typeof(EnhancedAudioClip))]
    public class EnhancedAudioTrack : Track
    {
        protected override Type GetBindingType()
        {
            return typeof(AudioSource);
        }


        protected override Playable CreateMixerPlayable(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<EnhancedAudioMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();
            mixer.TrackBinding = GenericTrackBinding;
            return playable;
        }

    }
}