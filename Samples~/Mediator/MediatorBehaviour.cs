using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


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
        [Mediator(false, true)] [SerializeField] private Medium m_onClipPlayMedium = new();
        [Header("While Clip Playing")]
        [Mediator(false, true)] [SerializeField] private Medium m_whileClipPlayingMedium = new();
        [Header("On Clip End")]
        [Mediator(false, true)] [SerializeField] private Medium m_onClipEndMedium = new();

        private PlayableDirector _director;

        private double _startTime = -1;
        private double _endTime;

        public TimelineClip TimelineClip { get; set; }


        public override void OnPlayableCreate(Playable playable)
        {
            _director = playable.GetGraph().GetResolver() as PlayableDirector;
        }


        public override void OnGraphStart(Playable playable)
        {
            StoreStartEndTimes();
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
            Mediator.Publish(m_onClipPlayMedium);
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Mediator.Publish(m_whileClipPlayingMedium);
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_director.time <= _startTime) // REQUIRED CHECK! OnBehaviourPause also runs right after OnGraphStart, so before this clip has actually played.
            {
                return;
            }

            Mediator.Publish(m_onClipEndMedium);
        }
    }
}