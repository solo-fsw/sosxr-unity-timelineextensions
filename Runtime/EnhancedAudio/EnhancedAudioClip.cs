using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class EnhancedAudioClip : Clip
    {
    
        public EnhancedAudioBehaviour Template;



        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EnhancedAudioBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();

            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            return playable;
        }


        /// <summary>
        ///     It's good practice to use this for anything in the Clip that needs setting up.
        ///     It gets called when the Clip is created from the Track.
        /// </summary>
        public override void InitializeClip()
        {
            if (Template == null)
            {
                return;
            }
            if (Template.Audio == null)
            {
                return;
            }
            
            if (Template.PreviousAudio == null || Template.PreviousAudio != Template.Audio)
            {
                TimelineClip.duration = Template.Audio.length;
                TimelineClip.displayName = Template.Audio.name;
                Template.PreviousAudio = Template.Audio;
            }

            /*if (TrackBinding != null)
            {
                Source = TrackBinding as AudioSource;
            }

            if (Source == null)
            {
                return;
            }
      
            if (TimelineClip.duration < Template.Audio.length || Math.Abs(TimelineClip.duration - Template.Audio.length) < 0.1f)
            {
                Source.loop = false;
                TimelineClip.duration = Template.Audio.length;
            }
            else
            {
                Source.loop = true;
            }*/
        }
    }
}