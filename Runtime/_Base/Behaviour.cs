using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable] // Also on the derived class, behaviours need to be serializable
    public class Behaviour : PlayableBehaviour
    {
        /// <summary>
        ///     Call this in the CreatePlayable method of the Clip.
        ///     If you need to override this method, always call this base method at the start of your override.
        /// </summary>
        public virtual void InitializeBehaviour(TimelineClip timelineClip, object trackBinding)
        {
            TimelineClip = timelineClip;
            TrackBinding = trackBinding;
        }


        #region Public Behaviour Properties

        public float EaseInDuration
        {
            get
            {
                if (TimelineClip == null)
                {
                    return 0;
                }

                return (float) TimelineClip.easeInDuration;
            }
        }

        public float EaseOutDuration
        {
            get
            {
                if (TimelineClip == null)
                {
                    return 0;
                }

                return (float) TimelineClip.easeOutDuration;
            }
        }


        public bool ClipStarted => ClipActive;

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

        public bool ClipActive { get; set; }

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

        public bool ClipEnd
        {
            get
            {
                if (_clipStarted && _clipIsDone) 
                {
                    ClipActive = false;

                    return true;
                }

                return false;
            }
        }

        public bool ClipIsDone
        {
            get => _clipIsDone;
            set
            {
                _clipIsDone = value;
                ClipActive = !value;
            }
        }

        #endregion

        #region Other Things

        /// <summary>
        ///     Use this to get the object that the Track is bound to.
        ///     You usually want to cast it to the specific type of your binding.
        /// </summary>
        public object TrackBinding { get; private set; }

        /// <summary>
        ///     This gets you information on the actual clip that's holding the Clip. Sorry, the naming is a little confusing.
        ///     Just note that this gets you information on the duration, easing times, playback speed, etc of the clip.
        /// </summary>
        public TimelineClip TimelineClip { get; private set; }


        /// <summary>
        ///     I'm hoping you don't need to override this any further, and that the public properties above are what you need in
        ///     the Mixer.
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public sealed override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _clipStarted = true;
            ClipActive = true;
        }


        /// <summary>
        ///     I'm hoping you don't need to override this any further, and that the public properties above are what you need in
        ///     the Mixer.
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        /// <param name="playerData"></param>
        public sealed override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _currentTime = (float) playable.GetTime();
        }


        /// <summary>
        ///     I'm hoping you don't need to override this any further, and that the public properties above are what you need in
        ///     the Mixer.
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public sealed override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (ClipActive)
            {
                _clipIsDone = true;
            }
        }

        #endregion

        #region Private

        private bool _clipStarted;
        private bool _clipStartedReported;
        private bool _easeInReported;
        private bool _easeOutReported;
        private float _currentTime;
        private bool _clipIsDone;

        private float _clipDuration
        {
            get
            {
                if (TimelineClip == null)
                {
                    Debug.LogError("TimelineClip is null, please make sure you've initialized the behaviour.");

                    return 0;
                }

                return (float) TimelineClip.duration;
            }
        }

        #endregion
    }
}