using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class TimeControlBehaviour : PlayableBehaviour
    {
        public ExposedReference<TimeControlBase> LoopBreakerReference;

        public TimeState InitialState; // This is what you set in the inspector for what this clip initially needs to do
        public TimeState CurrentState; // This allows us to revert back to choice made in inspector: otherwise this ScriptableObject will store the changes made in PlayMode

        public ITimeControl TimeControl;

        private double _startTime;
        private double _endTime;

        private bool _behaviourHasStarted;

        private PlayableDirector _director;
        private TimeState _previousTimeState; // This is to check whether we need to redraw the clip name during PlayMode
        private double _currentSpeed;
        private double _previousSpeed = -1;

        public TimeControlBase TimeControlBase { get; private set; }

        public TimelineClip TimelineClip { get; set; }

        public TimeControlClip TimeControlClip { get; set; }


        public override void OnPlayableCreate(Playable playable)
        {
            _director = playable.GetGraph().GetResolver() as PlayableDirector;

            if (_director == null)
            {
                return;
            }

            GetAndInitialiseLoopBreaker();
        }


        private void GetAndInitialiseLoopBreaker()
        {
            TimeControlBase = LoopBreakerReference.Resolve(_director);

            if (TimeControlBase == null)
            {
                LoopBreakerReference = new ExposedReference<TimeControlBase>();

                return;
            }

            TimeControlBase.TimeControl = this;
            TimeControlBase.Director = _director;

            if (TimeControlBase is ITimeControl timeControl)
            {
                TimeControl = timeControl;
            }

            _previousSpeed = -1;
        }


        public override void OnGraphStart(Playable playable)
        {
            SetCurrentState();
            RedrawClipNameIfLooperStateChanged();
            StoreStartEndTimes();
        }


        private void SetCurrentState()
        {
            CurrentState = InitialState;
            _previousSpeed = -1;
        }


        private void RedrawClipNameIfLooperStateChanged()
        {
            if (CurrentState == _previousTimeState)
            {
                return;
            }

            TimeControlClip.SetDisplayName(this, TimelineClip);
            _previousTimeState = CurrentState;
        }


        private void StoreStartEndTimes()
        {
            if (TimelineClip == null)
            {
                return;
            }

            _startTime = TimelineClip.start;
            _endTime = TimelineClip.end;
        }


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            _behaviourHasStarted = true;

            TimeControl?.OnControlTimeStart();
        }


        public override void PrepareFrame(Playable playable, FrameData info)
        {
            RedrawClipNameIfLooperStateChanged();
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (CurrentState == TimeState.TimeScaleZero)
            {
                _currentSpeed = 0;
            }
            else if (CurrentState == TimeState.Looping)
            {
                _currentSpeed = 1;
            }
            else if (CurrentState == TimeState.GoToStart)
            {
                _director.time = _startTime;
                CurrentState = TimeState.Continue;
                _currentSpeed = 1;
            }
            else if (CurrentState == TimeState.GoToEnd)
            {
                _director.time = _endTime;
                CurrentState = TimeState.Continue;
                _currentSpeed = 1;
            }
            else if (CurrentState == TimeState.Continue)
            {
                _currentSpeed = 1;
            }

            SetTimelineSpeed(_director, _currentSpeed);
        }


        public void SetTimelineSpeed(PlayableDirector director, double speed)
        {
            if (Math.Abs(_previousSpeed - speed) < 0.001)
            {
                return;
            }

            if (director.playableGraph.IsValid())
            {
                director.playableGraph.GetRootPlayable(0).SetSpeed(speed);

                _previousSpeed = speed;
            }
            else
            {
                Debug.LogWarning("Playable Graph is not valid, yet you're trying to access it! Is this on the last clip in the Timeline by any chance?");
            }
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_director.time <= _startTime) // REQUIRED CHECK! OnBehaviourPause also runs right after OnGraphStart, so before this clip has actually played.
            {
                return;
            }

            if (_behaviourHasStarted == false) // Backup check: making sure that the clip has started playing, therefore the below code doesn't run prior to clip start.
            {
                return;
            }

            if (CurrentState == TimeState.Looping)
            {
                _director.time = _startTime;
                _behaviourHasStarted = false;
            }

            TimeControl?.OnControlTimeStop();
        }


        public override void OnGraphStop(Playable playable)
        {
            SetCurrentState();
            RedrawClipNameIfLooperStateChanged();
        }
    }
}