using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track that binds to an <see cref="AudioSource"/> and creates <see cref="EnhancedAudioClip"/> clips.
    ///     Provides finer control over audio properties (volume, pitch, spatial blend, rolloff) than the built-in Audio track,
    ///     with ease-in/out acting as an automatic volume fade.
    /// </summary>
    [TrackColor(.506f, 0.435f, 0f)]
    [TrackBindingType(typeof(AudioSource))]
    [TrackClipType(typeof(EnhancedAudioClip))]
    public class EnhancedAudioTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            ScriptPlayable<EnhancedAudioMixer> playable = ScriptPlayable<EnhancedAudioMixer>.Create(graph, inputCount);

            var mixer = playable.GetBehaviour();

            if (mixer != null && TrackBinding is AudioSource audioSource)
            {
                mixer.TrackBinding = TrackBinding;

                mixer.Binding = audioSource;

                if (audioSource.playOnAwake || audioSource.mute)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        UnityEditor.Undo.RecordObject(audioSource, "Prepare AudioSource for Enhanced Audio track");
                    }
#endif
                    audioSource.playOnAwake = false;
                    audioSource.mute = false;
                }
            }

            return playable;
        }
    }
}
