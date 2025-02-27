using UnityEngine;
using UnityEngine.Playables;


/// <summary>
///     Based off of Unity Timeline sample pack : Time Dilation
///     From: https://docs.unity3d.com/Packages/com.unity.timeline@1.6/manual/smpl_about.html
/// </summary>
public class TimeScaleTrackMixer : PlayableBehaviour
{
    private float _oldTimeScale = 1f;


    public override void OnGraphStart(Playable playable)
    {
        _oldTimeScale = Time.timeScale;
    }


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var inputCount = playable.GetInputCount();

        var mixedTimeScale = 0f;
        var totalWeight = 0f;

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i);

            totalWeight += inputWeight;

            var playableInput = (ScriptPlayable<TimeScaleBehaviour>) playable.GetInput(i);
            var input = playableInput.GetBehaviour();

            mixedTimeScale += inputWeight * input.timeScale;
        }

        Time.timeScale = mixedTimeScale + _oldTimeScale * (1f - totalWeight);
    }


    public override void OnGraphStop(Playable playable)
    {
        Time.timeScale = _oldTimeScale;
    }
}