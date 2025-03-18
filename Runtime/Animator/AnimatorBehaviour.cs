using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This acts as our data for the clip to write to
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [Serializable]
    public class AnimatorBehaviour : PlayableBehaviour
    {
        public AnimatorClip AnimatorClip;
        public Animator TrackBinding;
        public string StartClipStateName = "";
        public float StartTransitionDuration = 0.25f;
        public string EndClipStateName = "Default_State";
        public float EndTransitionDuration;
        private bool _fadeStarted;

        private float _easeInTimer;


        public void Initialize(AnimatorClip animatorClip)
        {
            AnimatorClip = animatorClip;
            StartTransitionDuration = (float) AnimatorClip.TimelineClip.easeInDuration;
            EndTransitionDuration = (float) AnimatorClip.TimelineClip.easeOutDuration;
            _fadeStarted = false;
            _easeInTimer = 0;
        }


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (TrackBinding == null || AnimatorClip == null)
            {
                return;
            }

            if (!Application.isPlaying)
            {
                return;
            }

            if (string.IsNullOrEmpty(StartClipStateName) || StartClipStateName == "NONE")
            {
                return;
            }

            _fadeStarted = false;

            TrackBinding.CrossFade(StartClipStateName, StartTransitionDuration, 0);
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var animator = playerData as Animator;

            if (animator == null)
            {
                Debug.LogWarning("No animator found on the track");

                return;
            }

            TrackBinding ??= animator;

            _easeInTimer += Time.deltaTime;

            if (_easeInTimer <= StartTransitionDuration) // Ease in is still happening
            {
                return;
            }

            if (info.weight < 0.975f) // Ease Out is starting. Some value less than 1. I think this is safe.
            {
                ClipEnd();
            }
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (TrackBinding == null || AnimatorClip == null)
            {
                return;
            }

            if (!Application.isPlaying)
            {
                return;
            }

            ClipEnd(); // If no ease out has happened, it will here crossfade to the next animation. 
        }


        private void ClipEnd()
        {
            if (string.IsNullOrEmpty(EndClipStateName) || EndClipStateName == "NONE") // If we don't want to use the end clip state, we can just return. It will stay in the start clip state.
            {
                return;
            }

            if (_fadeStarted)
            {
                return;
            }

            TrackBinding.CrossFade(EndClipStateName, EndTransitionDuration, 0);

            _fadeStarted = true;
        }
    }
}