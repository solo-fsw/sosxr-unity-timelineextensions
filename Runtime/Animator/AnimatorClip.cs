using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     These variables allow us to set the value in the editor.
    /// </summary>
    public class AnimatorClip : Clip
    {
        public AnimatorBehaviour Template;

        [HideInInspector] public List<string> StateNames = new();

        [SerializeField] [HideInInspector] private Animator m_animator;


        public override void InitializeClip(object trackBinding, TimelineClip timelineClip, IExposedPropertyTable resolver)
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);

            m_animator = TrackBinding as Animator; // Cast the TrackBinding to the type of the binding. Don't do ??= here, because no.

            StateNames = m_animator?.GetStateNames();
            SetDisplayName();
        }


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            m_animator ??= TrackBinding as Animator;

            var playable = ScriptPlayable<AnimatorBehaviour>.Create(graph, Template);

            if (Template.EndClipStateName == "Default_State")
            {
                Template.EndClipStateName = m_animator.GetDefaultEntryStateName();
            }

            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            return playable;
        }


        /// <summary>
        ///     The displayName of the clip in Timeline will be set using this method.
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        private void SetDisplayName()
        {
            var displayName = "";

            if (!string.IsNullOrEmpty(Template.StartClipStateName))
            {
                displayName += "ClipStart: " + Template.StartClipStateName + " (" + Template.EaseInDuration + "s)";
            }

            if (!string.IsNullOrEmpty(Template.StartClipStateName) && !string.IsNullOrEmpty(Template.EndClipStateName))
            {
                displayName += " || ";
            }

            if (!string.IsNullOrEmpty(Template.EndClipStateName))
            {
                displayName += "ClipEnd: " + Template.EndClipStateName + " (" + Template.EaseOutDuration + "s)";
            }

            displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Clip");

            if (TimelineClip == null)
            {
                return;
            }

            TimelineClip.displayName = displayName;
        }


        [Button]
        private void MatchDurationToClips()
        {
            TimelineClip.easeOutDuration = m_animator.GetStateDuration(Template.EndClipStateName);
            TimelineClip.duration = m_animator.GetStateDuration(Template.StartClipStateName) + TimelineClip.easeOutDuration;
        }
    }
}