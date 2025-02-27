using UnityEngine;
using UnityEngine.Timeline;


/// <summary>
///     Use this base class if you want to give another object the power to break out of the loop in Timeline in some
///     fashion.
/// </summary>
public abstract class TimeControlBase : MonoBehaviour, ITimeControl
{
    [SerializeField] private TimeControlBehaviour m_timeControl;
    public TimeControlBehaviour TimeControl 
    {
        get => m_timeControl;
        set => m_timeControl = value;
    }


    [ContextMenu(nameof(Pause))]
    protected virtual void Pause()
    {
        TimeControl.CurrentState = TimeState.Pause;
    }


    [ContextMenu(nameof(Looping))]
    protected virtual void Looping()
    {
        TimeControl.CurrentState = TimeState.Looping;
    }


    [ContextMenu(nameof(Continue))]
    protected virtual void Continue()
    {
        TimeControl.CurrentState = TimeState.Continue;
    }


    [ContextMenu(nameof(BreakAndGoToStart))]
    protected virtual void BreakAndGoToStart()
    {
        TimeControl.CurrentState = TimeState.GoToStart;
    }


    [ContextMenu(nameof(BreakAndGoToEnd))]
    protected virtual void BreakAndGoToEnd()
    {
        TimeControl.CurrentState = TimeState.GoToEnd;
    }


    public virtual void SetTime(double time)
    {
        Debug.Log("SetTime: " + time);
    }


    public virtual void OnControlTimeStart()
    {
        Debug.Log("OnControlTimeStart");
    }


    public virtual void OnControlTimeStop()
    {
        Debug.Log("OnControlTimeStop");
    }
}