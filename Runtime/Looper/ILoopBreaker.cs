/// <summary>
///     Use this interface if you want to give another object the power to break out of the loop in Timeline in some
///     fashion.
/// </summary>
public interface ILoopBreaker
{
    public LooperBehaviour Looper { get; set; }


    /// <summary>
    ///     `Looper.RunningLooperState = LooperState.DoNotLoop;`
    /// </summary>
    public void BreakLoop();


    /// <summary>
    ///     `Looper.RunningLooperState = LooperState.GoToStart;`
    /// </summary>
    public void BreakAndGoToStart();


    /// <summary>
    ///     `Looper.RunningLooperState = LooperState.GoToEnd;`
    /// </summary>
    public void BreakAndGoToEnd();
}