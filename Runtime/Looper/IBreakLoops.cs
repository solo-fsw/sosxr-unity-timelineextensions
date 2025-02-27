using UnityEngine;


/// <summary>
///     Derive from this class if you want to give another object the power to break out of the loop in Timeline in some
///     fashion.
/// </summary>
public interface IBreakLoops
{
    public LooperBehaviour Looper { get; set; }


    public void Init(LooperBehaviour loopBehaviour)
    {
        if (loopBehaviour == null)
        {
            return;
        }

        Looper = loopBehaviour;
    }


    [ContextMenu(nameof(BreakLoop))]
    public void BreakLoop()
    {
        Looper.runningLooperState = LooperBehaviour.LooperState.DoNotLoop;
    }


    [ContextMenu(nameof(GoToStart))]
    public void GoToStart()
    {
        Looper.runningLooperState = LooperBehaviour.LooperState.GoToStart;
    }


    [ContextMenu(nameof(GoToEnd))]
    public void GoToEnd()
    {
        Looper.runningLooperState = LooperBehaviour.LooperState.GoToEnd;
    }
}