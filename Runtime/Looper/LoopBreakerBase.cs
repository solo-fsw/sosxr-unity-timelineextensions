using System;
using UnityEngine;


/// <summary>
///     Derive from this class if you want to give another object the power to break out of the loop in Timeline in some
///     fashion.
/// </summary>
[Serializable]
public abstract class LoopBreakerBase : MonoBehaviour
{
    protected LooperBehaviour looper;


    public void Init(LooperBehaviour loopBehaviour)
    {
        if (loopBehaviour == null)
        {
            return;
        }

        looper = loopBehaviour;
    }


    public void BreakLoop()
    {
        looper.runningLooperState = LooperBehaviour.LooperState.DoNotLoop;
    }


    public void GoToStart()
    {
        looper.runningLooperState = LooperBehaviour.LooperState.GoToStart;
    }


    public void GoToEnd()
    {
        looper.runningLooperState = LooperBehaviour.LooperState.GoToEnd;
    }
}