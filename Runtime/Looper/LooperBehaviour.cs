using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
public class LooperBehaviour : PlayableBehaviour
{
    public enum LooperState
    {
        Looping,
        GoToStart,
        GoToEnd,
        DoNotLoop
    }


    public bool handControlTo;

    public ExposedReference<LoopBreakerBase> loopBreakerReference; // This works for derived classes of LoopBreakerBase (since this Base is abstract)

    public LooperState startLooperState; // This is what you set in the inspector for what this clip initially needs to do
    public LooperState runningLooperState; // This allows us to revert back to choice made in inspector: otherwise this ScriptableObject will store the changes made in PlayMode

    private double startTime;
    private double endTime;

    private bool behaviourHasStarted;

    private PlayableDirector director;
    private LooperState oldLooperState; // This is to check whether we need to redraw the clip name during PlayMode

    public LoopBreakerBase LoopBreakerBase { get; private set; }

    public TimelineClip TimelineClip { get; set; }

    public LooperClip LooperClip { get; set; }


    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;

        if (director == null)
        {
            return;
        }

        GetAndInitialiseLoopBreakerBase();
    }


    private void GetAndInitialiseLoopBreakerBase()
    {
        if (handControlTo == false)
        {
            return;
        }

        LoopBreakerBase = loopBreakerReference.Resolve(director);

        if (LoopBreakerBase == null)
        {
            return;
        }

        LoopBreakerBase.Init(this);
    }


    public override void OnGraphStart(Playable playable)
    {
        SetRunningLooperState();
        RedrawClipNameIfLooperStateChanged();
        StoreStartEndTimes();
    }


    private void SetRunningLooperState()
    {
        runningLooperState = startLooperState;
    }


    private void RedrawClipNameIfLooperStateChanged()
    {
        if (runningLooperState == oldLooperState)
        {
            return;
        }

        LooperClip.SetDisplayName(this, TimelineClip);
        oldLooperState = runningLooperState;
    }


    private void StoreStartEndTimes()
    {
        if (TimelineClip == null)
        {
            return;
        }

        startTime = TimelineClip.start;
        endTime = TimelineClip.end;
    }


    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        behaviourHasStarted = true;
    }


    public override void PrepareFrame(Playable playable, FrameData info)
    {
        RedrawClipNameIfLooperStateChanged();
    }


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (runningLooperState == LooperState.Looping)
        {
            return;
        }

        if (runningLooperState == LooperState.GoToStart)
        {
            director.time = startTime;
        }
        else if (runningLooperState == LooperState.GoToEnd)
        {
            director.time = endTime;
        }

        runningLooperState = LooperState.DoNotLoop;
    }


    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (director.time <= startTime) // REQUIRED CHECK! OnBehaviourPause also runs right after OnGraphStart, so before this clip has actually played.
        {
            return;
        }

        if (behaviourHasStarted == false) // Backup check: making sure that the clip has started playing, therefore the below code doesn't run prior to clip start.
        {
            return;
        }

        if (runningLooperState == LooperState.Looping)
        {
            director.time = startTime;
            behaviourHasStarted = false;
        }
    }


    public override void OnGraphStop(Playable playable)
    {
        SetRunningLooperState();
        RedrawClipNameIfLooperStateChanged();
    }
}