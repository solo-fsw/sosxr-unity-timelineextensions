using System;
using UnityEngine;
using UnityEngine.Playables;


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

            clone.Audio = Audio;
            clone.Loop = _loop;

            return playable;
        }


        /// <summary>
        ///     It's good practice to use this for anything in the Clip that needs setting up.
        ///     It gets called when the Clip is created from the Track.
        /// </summary>
        public override void InitializeClip()
        {
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
    }
}