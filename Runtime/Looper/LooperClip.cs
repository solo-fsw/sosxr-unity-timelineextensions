using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the Looper track. Configures the initial <see cref="TimeState"/> for this clip in the Inspector.
    ///     The display name in Timeline reflects the current state with an icon for quick identification.
    /// </summary>
    [Serializable]
    public class LooperClip : Clip
    {
        [HideInInspector] public LooperBehaviour Template;

        public TimeState InitialState; // This is what you set in the inspector for what this clip initially needs to do

        public override ClipCaps clipCaps => ClipCaps.None; // Do not allow blending between clips

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Template.InitialState = InitialState;
            Template.CurrentState = InitialState;

            ScriptPlayable<LooperBehaviour> playable = ScriptPlayable<LooperBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip, TrackBinding);
            clone.SetDisplayName();

            return playable;
        }
    }
}
