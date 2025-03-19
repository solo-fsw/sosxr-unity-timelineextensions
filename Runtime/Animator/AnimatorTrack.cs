using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This creates the TrackMixer, and sets the name of the Clip.
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [TrackColor(0.745f, 0.414f, 0.255f)]
    [TrackBindingType(typeof(Animator))] // Bind to whatever you need to control in Timeline
    [TrackClipType(typeof(AnimatorClip))] // Tell the track that it can create clips from said binding
    public class AnimatorTrack : Track
    {
        protected override Type GetBindingType()
        {
            return typeof(Animator);
        }


        protected override Playable CreateMixerPlayable(PlayableGraph graph, int inputCount)
        {
            return ScriptPlayable<AnimatorMixer>.Create(graph, inputCount);
        }
    }
}