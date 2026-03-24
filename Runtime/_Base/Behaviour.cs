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
    [Serializable]
    public class Behaviour : PlayableBehaviour
    {
        #region Public Properties (Timeline Clip Info)

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

        /// <summary>True after the clip has finished playing (after <see cref="OnBehaviourPause"/> fires).</summary>
        public bool ClipIsDone { get; set; }

        /// <summary>True once the current playhead time has passed the ease-in duration (or if no ease-in).</summary>
        public bool EaseInDone => _currentTime >= EaseInDuration;

        /// <summary>True for exactly one frame the moment ease-in completes. Resets automatically on replay.</summary>
        /// <remarks>
        /// Backward compatibility: This property still works but internally uses the new state machine.
        /// Prefer subscribing to ClipEaseInDoneOnceAction in Mixers.
        /// </remarks>
        public bool EaseInDoneOnce => _easeInFired && !_easeInReportedThisFrame ? (_easeInReportedThisFrame = true) : false;

        /// <summary>True once the current playhead time has reached the ease-out window (or if no ease-out, at clip end).</summary>
        public bool EaseOutStarted => _currentTime >= _clipDuration - EaseOutDuration || ClipIsDone;

        /// <summary>True for exactly one frame the moment ease-out begins. Resets automatically on replay.</summary>
        /// <remarks>
        /// Backward compatibility: This property still works but internally uses the new state machine.
        /// Prefer subscribing to ClipEaseOutStartedOnceAction in Mixers.
        /// </remarks>
        public bool EaseOutStartedOnce => _easeOutOrFallbackFired && !_easeOutReportedThisFrame ? (_easeOutReportedThisFrame = true) : false;

        /// <summary>The current ease weight (0-1) set by the Mixer each frame.</summary>
        public float EaseWeight { get; set; }

        #endregion

        #region Event Actions

        /// <summary>Invoked once when the clip starts playing.</summary>
        public Action<Behaviour> ClipStartedAction;

        /// <summary>Invoked once the moment ease-in completes (or immediately if no ease-in).</summary>
        public Action<Behaviour> ClipEaseInDoneOnceAction;

        /// <summary>
        /// Invoked once when ease-out begins.
        /// If the clip ends before reaching ease-out, this fires as a fallback in OnBehaviourPause (exclusive-OR with natural ease-out).
        /// </summary>
        public Action<Behaviour> ClipEaseOutStartedOnceAction;

        /// <summary>Invoked once when the clip becomes inactive (end, pause, or interrupted).</summary>
        public Action<Behaviour> ClipEndedAction;

        #endregion

        #region References

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

        #endregion

        #region Initialization

        /// <summary>
        ///     Call this in the CreatePlayable method of the Clip.
        ///     If you need to override this method, always call this base method at the start of your override.
        /// </summary>
        public virtual void InitializeBehaviour(TimelineClip timelineClip, object trackBinding)
        {
            TimelineClip = timelineClip;
            TrackBinding = trackBinding;
        }

        #endregion

        #region PlayableBehaviour Lifecycle

        /// <summary>
        /// Called when the clip becomes active (playhead enters the clip).
        /// Resets state for this activation and fires ClipStartedAction.
        /// </summary>
        public sealed override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (ClipIsActive)
            {
                return;
            }

            ResetStateForActivation();

            ClipIsActive = true;
            ClipIsDone = false;

            _activationStartTime = playable.GetTime();
            _currentTime = 0f;
            _lastTime = -1.0;

            ClipStartedAction?.Invoke(this);

            double currentTime = playable.GetTime();
            EvaluateThresholds(currentTime, allowEndFallback: false, fireEvents: true);
        }

        /// <summary>
        /// Called each frame while the clip is active.
        /// Handles discontinuity detection (seek/loop) and evaluates phase thresholds.
        /// </summary>
        public sealed override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            double currentTimeDouble = playable.GetTime();
            _currentTime = (float)currentTimeDouble;

            bool discontinuity = DetectDiscontinuity(info, currentTimeDouble);

            if (discontinuity)
            {
                ReArmFlagsForTime(currentTimeDouble);
            }

            EvaluateThresholds(currentTimeDouble, allowEndFallback: false, fireEvents: true);

            _lastTime = currentTimeDouble;

            _easeInReportedThisFrame = false;
            _easeOutReportedThisFrame = false;
        }

        /// <summary>
        /// Called when the clip becomes inactive (playhead leaves, pause, or interruption).
        /// Handles the ease-out fallback (exclusive-OR semantics) and fires ClipEndedAction.
        /// </summary>
        public sealed override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (!ClipIsActive)
            {
                return;
            }

            if (!_easeOutOrFallbackFired)
            {
                _easeOutOrFallbackFired = true;
                ClipEaseOutStartedOnceAction?.Invoke(this);
            }

            ClipIsDone = true;
            ClipIsActive = false;

            ClipEndedAction?.Invoke(this);
        }

        #endregion

        #region Private State

        private bool _easeInFired;
        private bool _easeOutOrFallbackFired;
        private bool _easeInReportedThisFrame;
        private bool _easeOutReportedThisFrame;
        private double _lastTime = -1.0;
        private double _activationStartTime;
        private float _currentTime;

        #endregion

        #region Private Methods

        /// <summary>
        /// Resets all state flags for a new activation (clip start or loop).
        /// </summary>
        private void ResetStateForActivation()
        {
            _easeInFired = false;
            _easeOutOrFallbackFired = false;
            _easeInReportedThisFrame = false;
            _easeOutReportedThisFrame = false;
            _lastTime = -1.0;
        }

        private bool DetectDiscontinuity(FrameData info, double currentTime)
        {
            if (info.seekOccurred || info.timeLooped)
            {
                return true;
            }

            if (_lastTime >= 0 && currentTime < _lastTime - 0.0001)
            {
                return true;
            }

            if (_lastTime >= 0 && currentTime > _lastTime + 1.0)
            {
                return true;
            }

            return false;
        }

        private void ReArmFlagsForTime(double time)
        {
            if (EaseInDuration > 0 && time < EaseInDuration)
            {
                _easeInFired = false;
            }
            else if (EaseInDuration <= 0 || time >= EaseInDuration)
            {
                _easeInFired = true;
            }

            double easeOutStartTime = _clipDuration - EaseOutDuration;
            if (time < easeOutStartTime)
            {
                _easeOutOrFallbackFired = false;
            }
        }

        /// <summary>
        /// Evaluates phase thresholds and fires events.
        /// Handles the clamping logic for overlapping ease windows.
        /// </summary>
        /// <param name="time">Current clip local time</param>
        /// <param name="allowEndFallback">Whether to allow fallback firing at end (should only be true in OnBehaviourPause)</param>
        /// <param name="fireEvents">Whether to actually fire events or just update state</param>
        private void EvaluateThresholds(double time, bool allowEndFallback, bool fireEvents)
        {
            float easeInDur = EaseInDuration;
            float easeOutDur = EaseOutDuration;
            float clipDur = _clipDuration;

            double easeOutStartTime = clipDur - easeOutDur;
            bool overlap = easeInDur + easeOutDur > clipDur;

            if (!_easeInFired)
            {
                bool shouldFireEaseIn = false;

                if (easeInDur <= 0)
                {
                    shouldFireEaseIn = true;
                }
                else if (overlap && time >= easeOutStartTime)
                {
                    shouldFireEaseIn = true;
                }
                else if (time >= easeInDur)
                {
                    shouldFireEaseIn = true;
                }

                if (shouldFireEaseIn)
                {
                    _easeInFired = true;
                    if (fireEvents)
                    {
                        ClipEaseInDoneOnceAction?.Invoke(this);
                    }
                }
            }

            if (_easeInFired && !_easeOutOrFallbackFired && easeOutDur > 0)
            {
                if (time >= easeOutStartTime)
                {
                    _easeOutOrFallbackFired = true;
                    if (fireEvents)
                    {
                        ClipEaseOutStartedOnceAction?.Invoke(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the clip duration, with null check.
        /// </summary>
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
