using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the Enhanced Audio track. Assign an <see cref="AudioClip"/> and the clip duration will snap to the
    ///     audio length automatically. If the clip is extended beyond the audio length, it loops. Exposes a [Button] in the Inspector to reset
    ///     the duration to match the audio exactly.
    /// </summary>
    [Serializable]
    public class EnhancedAudioClip : Clip
    {
        public AudioClip Audio;

        [NoFoldOut]
        public EnhancedAudioBehaviour Template;
        private AudioClip _previousAudio;
        private bool _loop;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<EnhancedAudioBehaviour> playable = ScriptPlayable<EnhancedAudioBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            clone.Audio = Audio;
            clone.Loop = _loop;

            return playable;
        }

        public override void InitializeClip(object trackBinding, TimelineClip timelineClip, IExposedPropertyTable resolver)
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);

            if (Audio == null)
            {
                return;
            }

            if (_previousAudio == null || _previousAudio != Audio)
            {
                TimelineClip.duration = Audio.length;
                _previousAudio = Audio;
            }

            if (TimelineClip.duration <= Audio.length)
            {
                _loop = false;
                TimelineClip.duration = Audio.length;

                TimelineClip.displayName = Audio.name;
            }
            else if (TimelineClip.duration > Audio.length)
            {
                _loop = true;

                double numberOfLoops = Math.Round(TimelineClip.duration / Audio.length, 2);
                TimelineClip.displayName = Audio.name + " : (looping " + numberOfLoops + " times)";
            }

            if (Template != null)
            {
                Template.Loop = _loop;
            }
        }

        [Button]
        private void MatchDurationToClip()
        {
            if (TimelineClip != null && Audio != null)
            {
                return;
            }

            TimelineClip.duration = Audio.length;
        }
    }
}

