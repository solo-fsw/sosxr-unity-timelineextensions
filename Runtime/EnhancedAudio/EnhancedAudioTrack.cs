using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(.506f, 0.435f, 0f)]
    [TrackBindingType(typeof(AudioSource))]
    [TrackClipType(typeof(EnhancedAudioClip))]
    public class EnhancedAudioTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<EnhancedAudioMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();

            if (TrackBinding is AudioSource audioSource)
            {
                mixer.AudioSource = audioSource;
                mixer.AudioSource.playOnAwake = false;
                mixer.AudioSource.mute = false;
            }

            return playable;
        }
    }
}