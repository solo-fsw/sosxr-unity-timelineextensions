using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Base playable behaviour for all SOSXR Timeline Extension tracks.
    ///     Tracks easing state (ease-in done, ease-out started) and exposes lifecycle Actions that the Mixer subscribes to.
    ///     Extend this class for your own Timeline behaviour, then call <see cref="InitializeBehaviour"/> from your Clip's CreatePlayable.
    /// </summary>
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

        /// <summary>The ease-in duration in seconds, as set on the Timeline clip. Returns 0 if the clip has not been initialized.</summary>
        public float EaseInDuration
        {
            get
            {
                if (TimelineClip == null)
                {
                    return 0;
                }

                return (float)TimelineClip.easeInDuration;
            }
        }

        /// <summary>The ease-out duration in seconds, as set on the Timeline clip. Returns 0 if the clip has not been initialized.</summary>
        public float EaseOutDuration
        {
            get
            {
                if (TimelineClip == null)
                {
                    return 0;
                }

                return (float)TimelineClip.easeOutDuration;
            }
        }

        /// <summary>True while the clip is active (between <see cref="OnBehaviourPlay"/> and <see cref="OnBehaviourPause"/>).</summary>
        public bool ClipIsActive { get; set; }

        /// <summary>True once the current playhead time has passed the ease-in duration.</summary>
        // public bool EaseInDone => _currentTime >= EaseInDuration || (EaseInDuration >= _clipDuration && ClipIsDone);
        public bool EaseInDone => _currentTime >= EaseInDuration;
        public bool EaseInDoneByWeight => EaseWeight >= 1;

        public float EaseWeight { get; set; }

        /// <summary>True for exactly one frame the moment ease-in completes. Resets automatically.</summary>
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

        /// <summary>True once the current playhead time has reached the ease-out window.</summary>
        public bool EaseOutStarted => _currentTime >= _clipDuration - EaseOutDuration || ClipIsDone;

        /// <summary>True for exactly one frame the moment ease-out begins. Resets automatically.</summary>
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

        /// <summary>Invoked once when the clip starts playing.</summary>
        public Action<Behaviour> ClipStartedAction;
        /// <summary>Invoked once the moment ease-in completes.</summary>
        public Action<Behaviour> ClipEaseInDoneOnceAction;
        /// <summary>Invoked once the moment ease-out begins.</summary>
        public Action<Behaviour> ClipEaseOutStartedOnceAction;
        /// <summary>Invoked once when the clip ends.</summary>
        public Action<Behaviour> ClipEndedAction;

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

            if (!ClipIsActive)
            {
                ClipIsDone = false;
                ClipIsActive = true;
                ClipStartedAction?.Invoke(this);
                _easeOutReported = false;
            }
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

            _currentTime = (float)playable.GetTime();


            if (EaseInDoneOnce)
            {
                ClipEaseInDoneOnceAction?.Invoke(this);
            }

            if (EaseOutStartedOnce)
            {
                ClipEaseOutStartedOnceAction?.Invoke(this);
            }
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

            if (ClipIsActive)
            {
                ClipIsDone = true;
                ClipIsActive = false;
                ClipEndedAction?.Invoke(this);
            }
        }

        /// <summary>True after the clip has finished playing (after <see cref="OnBehaviourPause"/> fires).</summary>
        public bool ClipIsDone { get; set; }

        #endregion

        #region Private

        private bool _easeInReported;
        private bool _easeOutReported;
        private float _currentTime;
        private float _currentWeight;

        private float _clipDuration
        {
            get
            {
                if (TimelineClip == null)
                {
                    Debug.LogError("TimelineClip is null, please make sure you've initialized the behaviour.");

                    return 0;
                }

                return (float)TimelineClip.duration;
            }
        }

        #endregion
    }
}
