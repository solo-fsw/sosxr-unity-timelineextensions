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

        public TimelineClip TimelineClip { private get; set; }
        public Clip Clip { private get; set; }

        private float _currentTime { get; set; }
        private float _clipDuration => (float) TimelineClip.duration;

        public bool ClipStarted { get; set; }
        public bool ClipStartedOnce
        {
            get
            {
                if (ClipStarted && !_clipStartedReported)
                {
                    _clipStartedReported = true;

                    return true;
                }

                return false;
            }
        }
        public bool ClipIsDone { get; set; }
        public bool ClipIsDoneOnce
        {
            get
            {
                if (ClipIsDone && !_clipIsDoneReported)
                {
                    _clipIsDoneReported = true;

                    return true;
                }

                return false;
            }
        }
        
        private float _easeInDuration => (float) TimelineClip.easeInDuration;
        public bool EaseInHasFinished => _currentTime >= _easeInDuration || (_easeInDuration >= _clipDuration && ClipIsDone);
        public bool EaseInHasFinishedOnce
        {
            get
            {
                if (EaseInHasFinished && !_easeInReported)
                {
                    _easeInReported = true;

                    return true;
                }

                return false;
            }
        }


        private float _easeOutDuration => (float) TimelineClip.easeOutDuration;
        public bool EaseOutHasStarted => _currentTime >= _clipDuration - _easeOutDuration || (_easeOutDuration <= 0 && ClipIsDone) || (_easeOutDuration >= _clipDuration && ClipIsDone);
        public bool EaseOutHasStartedOnce
        {
            get
            {
                if (EaseOutHasStarted && !_easeOutReported)
                {
                    _easeOutReported = true;

                    return true;
                }

                return false;
            }
        }


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            ClipStarted = true;
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _currentTime = (float) playable.GetTime();
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (ClipStarted)
            {
                ClipIsDone = true;
            }
        }


        public void DisableClip()
        {
            ClipStarted = false;
        }
    }
}