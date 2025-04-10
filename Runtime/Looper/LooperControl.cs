using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Call these methods change the state of the Timeline
    ///     It supports buffering the state changes until the playhead is on the clip.
    /// </summary>
    public class LooperControl : MonoBehaviour
    {
        [Tooltip("In care this LooperControl is asked 'too early' to set the state of the Looper, we will buffer the state change until the playhead is on the clip. It will perform the action immediately when the playhead is on the clip, and then set this 'Buffered State' back to NONE.")]
        [DisableEditing] public TimeState BufferedState = TimeState.None;
        private LooperBehaviour _clipInTimeline;

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


        [ContextMenu(nameof(TimeScaleZero))]
        public void TimeScaleZero()
        {
            SetState(TimeState.TimeScaleZero);
        }


        [ContextMenu(nameof(Looping))]
        public void Looping()
        {
            SetState(TimeState.Looping);
        }


        [ContextMenu(nameof(BreakAndContinue))]
        public void BreakAndContinue()
        {
            SetState(TimeState.BreakAndContinue);
        }


        [ContextMenu(nameof(BreakAndGoToStart))]
        public void BreakAndGoToStart()
        {
            SetState(TimeState.BreakAndGoToStart);
        }


        [ContextMenu(nameof(BreakAndGoToEnd))]
        public void BreakAndGoToEnd()
        {
            SetState(TimeState.BreakAndGoToEnd);
        }


        private void SetState(TimeState state)
        {
            if (ClipInTimeline == null)
            {
                BufferedState = state;

                Debug.LogFormat("Playhead is not yet 'on' the clip, but we've already been asked to set the state of the (hopefully) upcoming clip. Therefore we're 'buffering' the {0} operation, and it will be performed by the LooperMixer as soon as the playhead is on the clip.", BufferedState);

                return;
            }

            ClipInTimeline.CurrentState = state;
            ClipInTimeline.SetDisplayName();
        }
    }


    public enum TimeState
    {
        None, // No state set. Only used to distinguish if we've 'buffered' a state change for later use.
        TimeScaleZero, // Timeline timescale is set to 0
        Looping, // Loop the loop
        BreakAndGoToStart, // Goes to the start, also breaks the loop
        BreakAndGoToEnd, // + breaks the loop
        BreakAndContinue // Stop looping, but continue onwards
    }
}