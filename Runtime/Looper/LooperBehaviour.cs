using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
public class LooperBehaviour : PlayableBehaviour
{
    public ExposedReference<GameObject> LoopBreakerObjectReference;

    public LooperState StartLooperState; // This is what you set in the inspector for what this clip initially needs to do
    public LooperState RunningLooperState; // This allows us to revert back to choice made in inspector: otherwise this ScriptableObject will store the changes made in PlayMode
    public GameObject LoopBreakerObject;

    private double _startTime;
    private double _endTime;

    private bool _behaviourHasStarted;

    private PlayableDirector _director;
    private LooperState _previousLooperState; // This is to check whether we need to redraw the clip name during PlayMode

    public ILoopBreaker LoopBreaker { get; private set; }

    public TimelineClip TimelineClip { get; set; }

    public LooperClip LooperClip { get; set; }


    public override void OnPlayableCreate(Playable playable)
    {
        _director = playable.GetGraph().GetResolver() as PlayableDirector;

        if (_director == null)
        {
            return;
        }

        GetAndInitialiseLoopBreakerBase();
    }


    private void GetAndInitialiseLoopBreakerBase()
    {
        LoopBreakerObject = LoopBreakerObjectReference.Resolve(_director);

        if (LoopBreakerObject == null)
        {
            Debug.LogError("No gameObject found");

            return;
        }

        LoopBreaker = LoopBreakerObject.GetComponent<ILoopBreaker>();

        if (LoopBreaker == null)
        {
            Debug.LogError($"Assigned object does not implement IBreakLoops: {LoopBreakerObject}", LoopBreakerObject);

            LoopBreakerObjectReference = new ExposedReference<GameObject>();

            return;
        }

        LoopBreaker.Looper = this;
    }


    public override void OnGraphStart(Playable playable)
    {
        SetRunningLooperState();
        RedrawClipNameIfLooperStateChanged();
        StoreStartEndTimes();
    }


    private void SetRunningLooperState()
    {
        RunningLooperState = StartLooperState;
    }


    private void RedrawClipNameIfLooperStateChanged()
    {
        if (RunningLooperState == _previousLooperState)
        {
            return;
        }

        LooperClip.SetDisplayName(this, TimelineClip);
        _previousLooperState = RunningLooperState;
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
    }


    public override void PrepareFrame(Playable playable, FrameData info)
    {
        RedrawClipNameIfLooperStateChanged();
    }


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (RunningLooperState == LooperState.Looping)
        {
            return;
        }

        if (RunningLooperState == LooperState.GoToStart)
        {
            _director.time = _startTime;
        }
        else if (RunningLooperState == LooperState.GoToEnd)
        {
            _director.time = _endTime;
        }

        RunningLooperState = LooperState.BreakLooping;
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

        if (RunningLooperState == LooperState.Looping)
        {
            _director.time = _startTime;
            _behaviourHasStarted = false;
        }
    }


    public override void OnGraphStop(Playable playable)
    {
        SetRunningLooperState();
        RedrawClipNameIfLooperStateChanged();
    }
}