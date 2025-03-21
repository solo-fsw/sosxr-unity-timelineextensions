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
        //  [HideInInspector] 
        [SerializeField] private string _previousStartClipStateName = "";
        //  [HideInInspector] 
        [SerializeField] private string _previousEndClipStateName = "";
        
        public AnimatorBehaviour Template;

        [HideInInspector] public List<string> StateNames = new();

        private Animator _animator;


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            _animator ??= TrackBinding as Animator;

            var playable = ScriptPlayable<AnimatorBehaviour>.Create(graph, Template);

            if (Template.EndClipStateName == "Default_State")
            {
                Template.EndClipStateName = _animator.GetDefaultEntryStateName();
            }

            var clone = playable.GetBehaviour();
            clone.InitializeBehaviour(TimelineClip, TrackBinding);

            StateNames = _animator.GetStateNames();

            SetDisplayName(TimelineClip, Template);

            Debug.Log("nandos");

            return playable;
        }


        /// <summary>
        ///     The displayName of the clip in Timeline will be set using this method.
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        private static void SetDisplayName(TimelineClip clip, AnimatorBehaviour template)
        {
            var displayName = "";

            if (!string.IsNullOrEmpty(template.StartClipStateName))
            {
                displayName += "ClipStart: " + template.StartClipStateName + " (" + template.EaseInDuration + "s)";
            }

            if (!string.IsNullOrEmpty(template.StartClipStateName) && !string.IsNullOrEmpty(template.EndClipStateName))
            {
                displayName += " || ";
            }

            if (!string.IsNullOrEmpty(template.EndClipStateName))
            {
                displayName += "ClipEnd: " + template.EndClipStateName + " (" + template.EaseOutDuration + "s)";
            }

            displayName = CustomPlayableClipHelper.SetDisplayNameIfStillEmpty(displayName, "New Clip");

            if (clip == null)
            {
                return;
            }

            clip.displayName = displayName;
        }

[ContextMenu(nameof(InitializeClip))]
        public override void InitializeClip()
        {
            #if UNITY_EDITOR
            _animator ??= TrackBinding as Animator;

            var startStateDuration = _animator.GetStateDuration(Template.StartClipStateName);
            var stateEndDuration = _animator.GetStateDuration(Template.EndClipStateName);
            
            if (string.IsNullOrEmpty(_previousEndClipStateName) || Template.EndClipStateName != _previousEndClipStateName)
            {
                //  TimelineClip.easeOutDuration = _animator.GetStateDuration(Template.EndClipStateName);
                
                _previousEndClipStateName = Template.EndClipStateName;
            }
            
            if (string.IsNullOrEmpty(_previousStartClipStateName) || Template.StartClipStateName != _previousStartClipStateName)
            {
                if (!_animator.IsLooping(Template.StartClipStateName))
                {
                    TimelineClip.duration = startStateDuration + TimelineClip.easeOutDuration;
                }

                _previousStartClipStateName = Template.StartClipStateName;
            }

            if (TimelineClip.duration - TimelineClip.easeOutDuration > startStateDuration && !_animator.IsLooping(Template.StartClipStateName))
            {
                Debug.Log("The clip duration is longer than the state duration, and the clip is not set to loop."); 
            }
            
            if (TimelineClip.easeOutDuration > stateEndDuration && !_animator.IsLooping(Template.EndClipStateName))
            {
                Debug.Log("The clip ease out duration is longer than the end state duration, and the clip is not set to loop."); 
            }
             
            TimelineClip.displayName = "StartClip: " + Template.StartClipStateName + " (" + Template.EaseInDuration + "s) || EndClip: " + Template.EndClipStateName + " (" + Template.EaseOutDuration + "s)";
            #endif

            Debug.Log("whin does this get called?");
        }


        public void MatchDurationToStartClip()
        {
            Debug.Log("MatchDurationToStartClip");
        }
    }
}