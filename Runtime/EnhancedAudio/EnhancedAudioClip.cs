using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class EnhancedAudioClip : Clip
    {
        public AudioClip Audio;
        [NoFoldOut] public EnhancedAudioBehaviour Template;
        private AudioClip _previousAudio;
        private bool _loop;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EnhancedAudioBehaviour>.Create(graph, Template);
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

                var numberOfLoops = Math.Round(TimelineClip.duration / Audio.length, 2);
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
            TimelineClip.duration = Audio.length;
        }
    }
}