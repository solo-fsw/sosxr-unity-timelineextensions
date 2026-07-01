using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     MonoBehaviour that controls the playback state of the Timeline at runtime by communicating with
    ///     <see cref="LooperMixer"/>. Call any of the public methods to change the <see cref="TimeState"/> of the
    ///     currently active <see cref="LooperClip"/>. State changes requested before the playhead reaches the clip are
    ///     buffered in <see cref="BufferedState"/> and applied automatically on arrival.
    /// </summary>
    public class LooperControl : MonoBehaviour
    {
        public bool AllowBuffering = true;

        [Tooltip("In case this LooperControl is asked 'too early' to set the state of the Looper, we will buffer the state change until the playhead is on the clip. It will perform the action immediately when the playhead is on the clip, and then set this 'Buffered State' back to NONE.")]
        [DisableEditing] public TimeState BufferedState = TimeState.None;

        private LooperBehaviour _clipInTimeline;

        /// <summary>
        ///     Set by <see cref="LooperMixer"/> when the playhead enters a clip. Setting this property also flushes any
        ///     <see cref="BufferedState"/> that was requested before the clip was active.
        /// </summary>
        public LooperBehaviour ClipInTimeline
        {
            get => _clipInTimeline;
            set
            {
                _clipInTimeline = value;

                if (BufferedState != TimeState.None)
                {
                    _clipInTimeline.CurrentState = BufferedState;
                    _clipInTimeline.SetDisplayName();

                    BufferedState = TimeState.None;
                }
            }
        }


        /// <summary>Sets the timeline speed to zero, effectively pausing the Director without pausing other game systems.</summary>
        [ContextMenu(nameof(TimeScaleZero))]
        public void TimeScaleZero()
        {
            SetState(TimeState.TimeScaleZero);
        }


        /// <summary>Enables looping: when the clip ends the playhead jumps back to the clip's start.</summary>
        [ContextMenu(nameof(Looping))]
        public void Looping()
        {
            SetState(TimeState.Looping);
        }


        /// <summary>Breaks the loop and lets the Timeline continue forward from the current position.</summary>
        [ContextMenu(nameof(BreakAndContinue))]
        public void BreakAndContinue()
        {
            SetState(TimeState.BreakAndContinue);
        }


        /// <summary>Breaks the loop and immediately jumps the playhead to the start of this clip.</summary>
        [ContextMenu(nameof(BreakAndGoToStart))]
        public void BreakAndGoToStart()
        {
            SetState(TimeState.BreakAndGoToStart);
        }


        /// <summary>Breaks the loop and immediately jumps the playhead to the end of this clip.</summary>
        [ContextMenu(nameof(BreakAndGoToEnd))]
        public void BreakAndGoToEnd()
        {
            SetState(TimeState.BreakAndGoToEnd);
        }


        private void SetState(TimeState state)
        {
            if (ClipInTimeline == null)
            {
                if (AllowBuffering)
                {
                    BufferedState = state;

                    Debug.Log("Playhead is not yet 'on' the clip, but we've already been asked to set the state of the (hopefully) upcoming clip. Therefore we're 'buffering' the " + BufferedState + " operation, and it will be performed by the LooperMixer as soon as the playhead is on the clip.");
                }
                else
                {
                    Debug.LogFormat("We're not yet 'on' the clip, but buffering is not allowed, so we won't yet set the future state of the upcoming clip");
                }

                return;
            }

            ClipInTimeline.CurrentState = state;
            ClipInTimeline.SetDisplayName();
        }
    }
}


/// <summary>Defines the playback state for a Looper clip.</summary>
public enum TimeState
{
    None, // No state set. Only used to distinguish if we've 'buffered' a state change for later use.
    TimeScaleZero, // Timeline timescale is set to 0
    Looping, // Loop the loop
    BreakAndGoToStart, // Goes to the start, also breaks the loop
    BreakAndGoToEnd, // + breaks the loop
    BreakAndContinue // Stop looping, but continue onwards
}
