using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
public class TimelineSpeedBehaviour : PlayableBehaviour
{
    public bool setSpeedAtStart;
    public double speedAtStart = 1;

    public bool setSpeedAtEnd;
    public double speedAtEnd;

    public bool handControlTo;
    public ExposedReference<TimelineResumer> resumerReference;
    public TimelineResumer resumerBase;
    private bool _clipPlayed;

    private PlayableDirector _director;
    private bool _endSpeedScheduled;

    public TimelineClip TimelineClip { get; set; }


    public override void OnPlayableCreate(Playable playable)
    {
        _director = playable.GetGraph().GetResolver() as PlayableDirector;

        if (_director == null)
        {
            return;
        }

        if (handControlTo == false)
        {
            return;
        }

        resumerBase = resumerReference.Resolve(_director);

        if (resumerBase != null)
        {
            resumerBase.Init(_director);
        }
    }


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_clipPlayed || !(info.weight > 0f))
        {
            return;
        }

        if (setSpeedAtEnd == false)
        {
            return;
        }

        if (speedAtEnd != 0 && speedAtEnd <= 0.01f && _endSpeedScheduled == false)
        {
            Debug.Log("Speed at end was <= 0.01f, we reset it to 0");
            speedAtEnd = 0f;
        }

        _endSpeedScheduled = true;

        _clipPlayed = true;
    }


    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (setSpeedAtStart == false)
        {
            return;
        }

        if (resumerBase != null)
        {
            if (resumerBase.Buffered)
            {
                resumerBase.Buffered = false;

                return;
            }

            resumerBase.TakeControl();
        }

        ExecutiveProducer.SetTimelineSpeed(_director, speedAtStart);
    }


    /// <summary>
    ///     Is also called when playhead moves off the current clip
    /// </summary>
    /// <param name="playable"></param>
    /// <param name="info"></param>
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_endSpeedScheduled == false)
        {
            return;
        }

        if (resumerBase != null)
        {
            if (resumerBase.Buffered)
            {
                resumerBase.Buffered = false;
                _endSpeedScheduled = false;
                _clipPlayed = false;

                return;
            }

            resumerBase.TakeControl();
        }

        ExecutiveProducer.SetTimelineSpeed(_director, speedAtEnd);

        _endSpeedScheduled = false;
        _clipPlayed = false;
    }
}