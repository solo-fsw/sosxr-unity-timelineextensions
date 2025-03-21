using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Use this base class if you want to give another object the power to break out of the loop / pause in Timeline in
    ///     some fashion.
    /// </summary>
    public class TimelineControl : MonoBehaviour
    {
        public TimeControlBehaviour TimeControl { get; set; }


        [Button]
        [ContextMenu(nameof(TimeScaleZero))]
        private void TimeScaleZero()
        {
            TimeControl.CurrentState = TimeState.TimeScaleZero;
            TimeControl.SetDisplayName();
        }


        [Button]
        [ContextMenu(nameof(Looping))]
        protected virtual void Looping()
        {
            TimeControl.CurrentState = TimeState.Looping;
            TimeControl.SetDisplayName();
        }


        [Button]
        [ContextMenu(nameof(Continue))]
        protected virtual void Continue()
        {
            TimeControl.CurrentState = TimeState.Continue;
            TimeControl.SetDisplayName();
        }


        [Button]
        [ContextMenu(nameof(BreakAndGoToStart))]
        protected virtual void BreakAndGoToStart()
        {
            TimeControl.CurrentState = TimeState.GoToStart;
            TimeControl.SetDisplayName();
        }


        [Button]
        [ContextMenu(nameof(BreakAndGoToEnd))]
        protected virtual void BreakAndGoToEnd()
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