using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    public class AnimatorMixer : PlayableBehaviour
    {
        private float _previousWeight = -1;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var trackBinding = playerData as Animator;

            if (trackBinding == null)
            {
                return;
            }

            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var weight = playable.GetInputWeight(i);
                
                if (weight <= 0) // If the weight is 0, then this behaviour is not active
                {
                    continue;
                }

                var playableInput = (ScriptPlayable<AnimatorBehaviour>) playable.GetInput(i);
                var behaviour = playableInput.GetBehaviour();

                if (behaviour == null)
                {
                    continue;
                }

                //ActiveBehaviour(behaviour, trackBinding, weight);
            }
        }


        private void ActiveBehaviour(AnimatorBehaviour activeBehaviour, Animator trackBinding, float weight)
        {
            if (!activeBehaviour.StartCrossFadeStarted)
            {
                trackBinding.CrossFade(activeBehaviour.StartClipStateName, activeBehaviour.StartTransitionDuration, 0);
                activeBehaviour.StartCrossFadeStarted = true;
            }

            if (!EaseOutHasStarted(weight))
            {
                return;
            }

            if (!activeBehaviour.EndCrossFadeStarted)
            {
                trackBinding.CrossFade(activeBehaviour.EndClipStateName, activeBehaviour.EndTransitionDuration, 0);
                activeBehaviour.EndCrossFadeStarted = true;
                
                Debug.LogWarning("End CrossFade Started");

                _previousWeight = -1; // Reset the previous weight so that the next time the clip is active, the ease out will start again
            }
        }


        private bool EaseOutHasStarted(float weight)
        {
            var easingIn = weight >= _previousWeight; // Every frame, the weight during ease in should be greater than the previous frame
            var atMaxWeight = weight >= 1;

            if (easingIn || atMaxWeight)
            {
                _previousWeight = weight;

                return false;
            }

            return true;
        }
    }
}