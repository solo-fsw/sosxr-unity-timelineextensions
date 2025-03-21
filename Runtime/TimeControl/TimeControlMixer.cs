using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class TimeControlMixer : Mixer
    {
        private double _previousSpeed = -1;
        public PlayableDirector Director { get; set; }
        public TimelineControl TimelineControl { get; set; }


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not TimeControlBehaviour behaviour)
            {
                return;
            }

            if (Director == null)
            {
                Debug.LogWarning("Director is null, did you forget to set it?");

                return;
            }

            if (Director.state != PlayState.Playing)
            {
                Debug.LogWarning("Director is not playing, did you forget to play it?");

                return;
            }

            TimelineControl ??= TrackBinding as TimelineControl;

            if (TimelineControl == null)
            {
                Debug.LogWarning("TimelineControl is null, did you forget to set it?");

                return;
            }

            if (behaviour.ClipStartedOnce)
            {
                TimelineControl.TimeControl = behaviour;
            }

            if (behaviour.CurrentState == TimeState.GoToStart)
            {
                Director.time = behaviour.TimelineClip.start;
                behaviour.CurrentState = TimeState.Continue;
            }
            else if (behaviour.CurrentState == TimeState.GoToEnd)
            {
                Director.time = behaviour.TimelineClip.end;
                behaviour.CurrentState = TimeState.Continue;
            }

            var timelineSpeed = behaviour.CurrentState == TimeState.TimeScaleZero ? 0 : 1;
            SetTimelineSpeed(timelineSpeed);

            if (behaviour.ClipEnd && behaviour.CurrentState is TimeState.Looping or TimeState.GoToStart)
            {
                Director.time = behaviour.TimelineClip.start;
                behaviour.ClipIsDone = false;
            }
        }


        public void SetTimelineSpeed(double speed)
        {
            if (Math.Abs(_previousSpeed - speed) < 0.001)
            {
                return;
            }

            if (!Director.playableGraph.IsValid())
            {
                Debug.LogWarning("Playable Graph is not valid, yet you're trying to access it! Is this on the last clip in the Timeline by any chance?");

                return;
            }

            Director.playableGraph.GetRootPlayable(0).SetSpeed(speed);

            _previousSpeed = speed;
        }
    }
}