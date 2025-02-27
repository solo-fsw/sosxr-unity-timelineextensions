using System;
using UnityEngine.Playables;


/// <summary>
///     Based off of Unity Timeline sample pack : Time Dilation
///     From: https://docs.unity3d.com/Packages/com.unity.timeline@1.6/manual/smpl_about.html
/// </summary>
[Serializable]
public class TimeScaleBehaviour : PlayableBehaviour
{
    public float timeScale = 1f;
}