public enum LooperState
{
    Looping,
    GoToStart, // Also breaks the loop
    GoToEnd, // Also breaks the loop
    BreakLooping // Stop looping, but continue onwards
}