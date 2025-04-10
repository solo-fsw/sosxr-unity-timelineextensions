using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class LooperMixer : Mixer
    {
        private double _previousSpeed = -1;
        public PlayableDirector Director { get; set; }
        public LooperControl LooperControl { get; set; }


        protected override void InitializeMixer(Playable playable)
        {
            LooperControl ??= TrackBinding as LooperControl;

            if (Director == null)
            {
                Debug.LogWarning("Director is null, did you forget to set it?");
            }

            if (!Director.playableGraph.IsValid())
            {
                Debug.LogWarning("Playable Graph is not valid, yet you're trying to access it! Is this on the last clip in the Timeline by any chance?");
            }
        }


        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            var behaviour = activeBehaviour as LooperBehaviour;

            LooperControl.ClipInTimeline = behaviour;
        }


        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            var behaviour = activeBehaviour as LooperBehaviour;

            if (behaviour.CurrentState == TimeState.BreakAndGoToStart)
            {
                Director.time = behaviour.TimelineClip.start;
                behaviour.CurrentState = TimeState.BreakAndContinue;
            }
            else if (behaviour.CurrentState == TimeState.BreakAndGoToEnd)
            {
                Director.time = behaviour.TimelineClip.end;
                behaviour.CurrentState = TimeState.BreakAndContinue;
            }

            var timelineSpeed = behaviour.CurrentState == TimeState.TimeScaleZero ? 0 : 1;
            SetTimelineSpeed(timelineSpeed);
        }


        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            var behaviour = activeBehaviour as LooperBehaviour;

            if (behaviour.CurrentState is TimeState.Looping or TimeState.BreakAndGoToStart) // We need to do this clip again
            {
                Director.time = behaviour.TimelineClip.start;
                behaviour.ClipIsDone = false;
            }
            else // We are done with this clip
            {
                LooperControl.ClipInTimeline = null;
                behaviour.ClipIsDone = true;
            }
        }


        public void SetTimelineSpeed(double speed)
        {
            if (Math.Abs(_previousSpeed - speed) < 0.001)
            {
                return;
            }

            /*if (!Director.playableGraph.IsValid())
            {
                Debug.LogWarning("Playable Graph is not valid, yet you're trying to access it! Is this on the last clip in the Timeline by any chance?");

                return;
            }*/

            Director.playableGraph.GetRootPlayable(0).SetSpeed(speed);

            _previousSpeed = speed;
        }
    }
}