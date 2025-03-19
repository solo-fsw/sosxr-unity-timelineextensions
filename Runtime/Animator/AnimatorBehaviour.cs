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
        public string StartClipStateName = "";

        public string EndClipStateName = "Default_State";

        private float _previousWeight = -1;
        public Animator TrackBinding { get; set; }
        public float StartTransitionDuration { get; set; }
        public bool StartCrossFadeStarted { get; set; }
        public float EndTransitionDuration { get; set; }
        public bool EndCrossFadeStarted { get; set; }


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            TrackBinding.CrossFade(StartClipStateName, StartTransitionDuration, 0);
            StartCrossFadeStarted = true;
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (!StartCrossFadeStarted)
            {
                return;
            }

            if (!EndCrossFadeStarted)
            {
                return;
            }

            var currentWeight = playable.GetInputWeight(0);

            if (currentWeight >= _previousWeight) // easing in
            {
                _previousWeight = currentWeight;

                return;
            }

            if (currentWeight >= 1) // max weight
            {
                return;
            }

            // easing out

            TrackBinding.CrossFade(EndClipStateName, EndTransitionDuration, 0);
            EndCrossFadeStarted = true;
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (!StartCrossFadeStarted)
            {
                return;
            }

            if (!EndCrossFadeStarted)
            {
                return;
            }

            TrackBinding.CrossFade(EndClipStateName, EndTransitionDuration, 0);
            EndCrossFadeStarted = true;
        }
    }
}