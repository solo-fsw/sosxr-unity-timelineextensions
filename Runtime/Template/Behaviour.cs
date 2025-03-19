using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class Behaviour : PlayableBehaviour
    {
        public Transform Example; // Data is stored here

        private bool _clipStartedReported;
        private bool _easeInReported;
        private bool _easeOutReported;
        private bool _clipIsDoneReported;
        private float _currentTime;
        private bool _clipIsDone;


        public TimelineClip TimelineClip { private get; set; }
        private float _clipDuration => (float) TimelineClip.duration;
        private float _easeInDuration => (float) TimelineClip.easeInDuration;
        private float _easeOutDuration => (float) TimelineClip.easeOutDuration;

        public bool ClipHasStarted
        {
            get
            {
                if (ClipIsActive && !_clipStartedReported)
                {
                    _clipStartedReported = true;

                    return true;
                }

                return false;
            }
        }

        public bool ClipIsActive { get; private set; }

        public bool EaseInDone => _currentTime >= _easeInDuration || (_easeInDuration >= _clipDuration && _clipIsDone);

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

        public bool EaseOutStarted => _currentTime >= _clipDuration - _easeOutDuration || (_easeOutDuration <= 0 && _clipIsDone) || (_easeOutDuration >= _clipDuration && _clipIsDone);

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


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            ClipIsActive = true;
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _currentTime = (float) playable.GetTime();
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (ClipIsActive)
            {
                _clipIsDone = true;
            }
        }
    }
}