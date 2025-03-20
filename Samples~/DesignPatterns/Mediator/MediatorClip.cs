using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Allows us to set the values in the editor
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [Serializable]
    public class MediatorClip : PlayableAsset, ITimelineClipAsset
    {
        public MediatorBehaviour Template = new();

        public ClipCaps clipCaps => ClipCaps.None;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<MediatorBehaviour>.Create(graph, Template);

            return playable;
        }
    }
}