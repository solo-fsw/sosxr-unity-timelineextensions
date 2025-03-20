using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class Behaviour : PlayableBehaviour
    {
        #region Suggested to override in the implementation

        /// <summary>
        ///     It's good practice to call this when creating the Behaviour from the Clip in the CreatePlayable method.
        ///     Unfortunately you have to do that manually, in the CreatePlayable method of the Clip.
        ///     See the ExampleBehaviour and the ExampleClip for examples.
        /// </summary>
        public virtual void InitializeBehaviour(TimelineClip timelineClip, object trackBinding)
        {
            TimelineClip = timelineClip;
            TrackBinding = trackBinding;
        }

        #endregion

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

        #endregion

        #region Other Things

        /// <summary>
        ///     Use this to get the object that the Track is bound to.
        ///     You usually want to cast it to the specific type of your binding.
        /// </summary>
        public object TrackBinding { get; set; }

        /// <summary>
        ///     This gets you information on the actual clip that's holding the Clip. Sorry, the naming is a little confusing.
        ///     Just note that this gets you information on the duration, easing times, playback speed, etc of the clip.
        /// </summary>
        public TimelineClip TimelineClip { private get; set; }


        /// <summary>
        ///     I'm hoping you don't need to override this any further, and that the public properties above are what you need in
        ///     the Mixer.
        ///     However, if you do override this: always implement this base when overriding OnBehaviourPlay, otherwise the helper
        ///     properties won't work anymore.
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            ClipIsActive = true;
        }


        /// <summary>
        ///     I'm hoping you don't need to override this any further, and that the public properties above are what you need in
        ///     the Mixer.
        ///     However, if you do override this: always implement this base when overriding this ProcessFrame, with
        ///     base.ProcessFrame(playable, info, playerData) at the START of the method
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        /// <param name="playerData"></param>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
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
        ///     However, if you do override this: always implement this base when overriding OnBehaviourPause
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            if (ClipIsActive)
            {
                _clipIsDone = true;
            }
        }

        #endregion

        #region Private

        private bool _clipStartedReported;
        private bool _easeInReported;
        private bool _easeOutReported;
        private bool _clipIsDoneReported;
        private float _currentTime;
        private bool _clipIsDone;

        private float _clipDuration
        {
            get
            {
                if (TimelineClip == null)
                {
                    return 0;
                }

                return (float) TimelineClip.duration;
            }
        }

        #endregion
    }
}