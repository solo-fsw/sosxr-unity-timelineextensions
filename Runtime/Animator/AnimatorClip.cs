using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    public class AnimatorClip : Clip
    {
        public AnimatorBehaviour Template;
        [HideInInspector] public List<string> StateNames = new();
        [SerializeField][HideInInspector] private Animator m_animator;

        public override void InitializeClip(object trackBinding, TimelineClip timelineClip, IExposedPropertyTable resolver)
        {
            base.InitializeClip(trackBinding, timelineClip, resolver);
            m_animator = TrackBinding as Animator;

#if UNITY_EDITOR
            StateNames = m_animator?.GetStateNames();
            SetDisplayName();
#endif
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            m_animator ??= TrackBinding as Animator;

            ScriptPlayable<AnimatorBehaviour> playable = ScriptPlayable<AnimatorBehaviour>.Create(graph, Template);
            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip, TrackBinding);
            return playable;
        }

        private void SetDisplayName()
        {
            if (TimelineClip == null || Template == null)
            {
                return;
            }

            string stateName = Template.StateName;

            if (string.IsNullOrEmpty(stateName))
            {
                stateName = "Null";
            }

            TimelineClip.displayName = $"{stateName}";
        }
    }
}
