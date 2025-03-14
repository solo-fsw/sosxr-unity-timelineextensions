using System;
using SOSXR.SeaShark.Patterns.Mediator;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Acts as our data for the clip to write to
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [Serializable]
    public class MediatorBehaviour : PlayableBehaviour
    {
        [Header("On Clip Play")]
        [SerializeField] private Object m_thing;
        [Mediator(true )] [SerializeField] private Medium m_onClipPlay;
        [Header("While Clip Playing")]
        [SerializeField] private string m_thingWhileClipPlaying;
        [Mediator(true)] [SerializeField] private Medium m_whileClipPlaying;
        [Header("On Clip End")]
        [SerializeField] private bool m_thingOnClipEnd;
        [Mediator(true)] [SerializeField] private Medium m_onClipEnd;

        private PlayableDirector _director;

        private double _startTime = -1;
        private double _endTime;

        private bool _hasStarted;

        public TimelineClip TimelineClip { get; set; }


        public override void OnPlayableCreate(Playable playable)
        {
            _director = playable.GetGraph().GetResolver() as PlayableDirector;
        }


        public override void OnGraphStart(Playable playable)
        {
            _hasStarted = false;

            if (TimelineClip == null)
            {
                return;
            }

            _startTime = TimelineClip.start;
            _endTime = TimelineClip.end;
        }

     
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_onClipPlay.Data = m_thing;
            Mediator.Publish(m_onClipPlay);
            _hasStarted = true;
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (_director.time < _startTime || _director.time > _endTime)
            {
                return;
            }
            
            m_whileClipPlaying.Data = m_thingWhileClipPlaying;

            Mediator.Publish(m_whileClipPlaying);
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_director.time <= _startTime) // REQUIRED CHECK! OnBehaviourPause also runs right after OnGraphStart, so before this clip has actually played.
            {
                return;
            }

            if (!_hasStarted)
            {
                return;
            }

            m_onClipEnd.Data = m_thingOnClipEnd;
            Mediator.Publish(m_onClipEnd);
        }
    }
}