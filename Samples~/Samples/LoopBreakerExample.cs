using UnityEngine;


/// <summary>
///     This is an example of how you can use the ILoopBreaker interface to break out of a loop in Timeline.
/// </summary>
public class LoopBreakerExample : MonoBehaviour, ILoopBreaker
{
    [SerializeField] private LooperBehaviour looper;
    public LooperBehaviour Looper { get => looper; set => looper = value; }


    [ContextMenu(nameof(BreakLoop))]
    public void BreakLoop()
    {
        Looper.RunningLooperState = LooperState.BreakLooping;
        Debug.Log("Loop broken");
    }


    [ContextMenu(nameof(BreakAndGoToStart))]
    public void BreakAndGoToStart()
    {
        Looper.RunningLooperState = LooperState.GoToStart;
        Debug.Log("Going to start");
    }


    [ContextMenu(nameof(BreakAndGoToEnd))]
    public void BreakAndGoToEnd()
    {
        Looper.RunningLooperState = LooperState.GoToEnd;
        Debug.Log("Going to end");
    }
}