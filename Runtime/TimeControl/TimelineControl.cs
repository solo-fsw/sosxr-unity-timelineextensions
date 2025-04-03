using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Call these methods change the state of the Timeline
    ///     Keep in mind that these change te state of the _current_ clip... so that if the Timeline is not yet 'on' the clip, it will not work.
    ///     There is no "buffering" of the state changes (yet).
    /// </summary>
    public class TimelineControl : MonoBehaviour
    {
        public TimeControlBehaviour TimeControl { get; set; }


        [ContextMenu(nameof(TimeScaleZero))]
        public void TimeScaleZero()
        {
            TimeControl.CurrentState = TimeState.TimeScaleZero;
            TimeControl.SetDisplayName();
        }


        [ContextMenu(nameof(Looping))]
        public void Looping()
        {
            TimeControl.CurrentState = TimeState.Looping;
            TimeControl.SetDisplayName();
        }


        [ContextMenu(nameof(Continue))]
        public void Continue()
        {
            TimeControl.CurrentState = TimeState.Continue;
            TimeControl.SetDisplayName();
        }


        [ContextMenu(nameof(BreakAndGoToStart))]
        public void BreakAndGoToStart()
        {
            TimeControl.CurrentState = TimeState.GoToStart;
            TimeControl.SetDisplayName();
        }


        [ContextMenu(nameof(BreakAndGoToEnd))]
        public void BreakAndGoToEnd()
        {
            TimeControl.CurrentState = TimeState.GoToEnd;
            TimeControl.SetDisplayName();
        }
    }


    public enum TimeState
    {
        TimeScaleZero, // Timeline timescale is set to 0
        Looping, // Loop the loop
        GoToStart, // Goes to the start, also breaks the loop
        GoToEnd, // + breaks the loop
        Continue // Stop looping, but continue onwards
    }
}