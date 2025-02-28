using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    
    [TrackColor(.8f, 0.6f, 0f)]
    [TrackBindingType(typeof(AudioSource))]
    [TrackClipType(typeof(EnhancedAudioClip))]
    public class EnhancedAudioTrack : TrackAsset
    {
        
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
        {
            if (!graph.IsValid())
            {
                throw new ArgumentException("graph must be a valid PlayableGraph");
            }

            if (clip == null)
            {
                throw new ArgumentNullException(nameof(clip));
            }

            if (clip.asset is not IPlayableAsset asset)
            {
                return Playable.Null;
            }

            var handle = asset.CreatePlayable(graph, go);

            if (!handle.IsValid())
            {
                return handle;
            }

            handle.SetAnimatedProperties(clip.curves);
            handle.SetSpeed(clip.timeScale);

            if (clip.asset is not EnhancedAudioClip enhancedAudioClip)
            {
                return handle;
            }

            enhancedAudioClip.TimelineClip = clip;
            enhancedAudioClip.Template.TrackBinding = (AudioSource) go.GetComponent<PlayableDirector>().GetGenericBinding(this);

            if (enhancedAudioClip.Clip == null)
            {
                return handle;
            }

            if (clip.duration < enhancedAudioClip.Clip.length || Math.Abs(clip.duration - enhancedAudioClip.Clip.length) < 0.1f)
            {
                clip.duration = enhancedAudioClip.Clip.length;
            }
            else
            {
                if (enhancedAudioClip.Template != null && enhancedAudioClip.Template.TrackBinding != null)
                {
                    enhancedAudioClip.Template.TrackBinding.loop = true;
                }
            }

            return handle;
        }


        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<EnhancedAudioTrackMixer>.Create(graph, inputCount);
        }


        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            if (clip.asset is not EnhancedAudioClip enhancedAudioClip)
            {
                return;
            }

            if (enhancedAudioClip.Clip == null)
            {
                return;
            }

            if (clip.duration < enhancedAudioClip.Clip.length || Math.Abs(clip.duration - enhancedAudioClip.Clip.length) < 0.1f)
            {
                clip.duration = enhancedAudioClip.Clip.length;
            }
            else
            {
                if (enhancedAudioClip.Template != null && enhancedAudioClip.Template.TrackBinding != null)
                {
                    enhancedAudioClip.Template.TrackBinding.loop = true;
                }
            }
        }
    }
}