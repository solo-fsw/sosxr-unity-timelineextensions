using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class Behaviour : PlayableBehaviour
    {
        private bool _clipStartedReported;
        private bool _easeInReported;
        private bool _easeOutReported;
        private bool _clipIsDoneReported;
        private float _currentTime;
        private bool _clipIsDone;

        public object TrackBinding { get; set; }
        public TimelineClip TimelineClip { private get; set; }
        private float _clipDuration => (float) TimelineClip.duration;
        public float EaseInDuration => (float) TimelineClip.easeInDuration;
        public float EaseOutDuration => (float) TimelineClip.easeOutDuration;


        public bool ClipHasStarted => ClipIsActive;

        public bool ClipHasStartedOnce
        {
            get
            {
                if (ClipHasStarted && !_clipStartedReported)
                {
                    _clipStartedReported = true;

                    return true;
                }

                return false;
            }
        }

        public bool ClipIsActive { get; private set; }

        public bool EaseInDone => _currentTime >= EaseInDuration || (EaseInDuration >= _clipDuration && _clipIsDone);

        public bool EaseInDoneOnce
        {
            get
            {
                if (EaseInDone && !_easeInReported)
                {
                    _easeInReported = true;

                    return true;
                }

                return false;
            }
        }

        public bool EaseOutStarted => _currentTime >= _clipDuration - EaseOutDuration || (EaseOutDuration <= 0 && _clipIsDone) || (EaseOutDuration >= _clipDuration && _clipIsDone);

        public bool EaseOutStartedOnce
        {
            get
            {
                if (EaseOutStarted && !_easeOutReported)
                {
                    _easeOutReported = true;

                    return true;
                }

                return false;
            }
        }

        public bool ClipIsDone
        {
            get
            {
                if (_clipIsDone && !_clipIsDoneReported)
                {
                    ClipIsActive = false;
                    _clipIsDoneReported = true;

                    return true;
                }

                return false;
            }
        }


        /// <summary>
        ///     Always implement this base when overriding OnBehaviourPlay
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            ClipIsActive = true;
        }


        /// <summary>
        ///     Always implement this base when overriding this ProcessFrame, with base.ProcessFrame(playable, info, playerData) at
        ///     the START of the method
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        /// <param name="playerData"></param>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _currentTime = (float) playable.GetTime();
        }


        /// <summary>
        ///     Always implement this base when overriding OnBehaviourPause
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (ClipIsActive)
            {
                _clipIsDone = true;
            }
        }


        public virtual void InitializeBehaviour()
        {
        }
    }
}