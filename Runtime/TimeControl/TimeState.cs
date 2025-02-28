namespace SOSXR.TimelineExtensions
{
    public enum TimeState
    {
        TimeScaleZero, // Timeline timescale is set to 0
        Looping,
        GoToStart, // Also breaks the loop
        GoToEnd, // Also breaks the loop
        Continue // Stop looping, but continue onwards
    }
}