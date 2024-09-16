using UnityEngine;
using UnityEngine.Playables;


/// <summary>
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
public class LightsTrackMixer : PlayableBehaviour
{
    private int previousIndex = -1;


    /// <summary>
    ///     Tell playable what to do when the playhead is on this clip
    /// </summary>
    /// <param name="playable"></param>
    /// <param name="info"></param>
    /// <param name="playerData"></param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var data = (Light) playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

        if (!data)
        {
            return;
        }

        SetLightFromCurrentClipValue(playable, data);
    }


    private void SetLightFromCurrentClipValue(Playable playable, Light data)
    {
        var inputCount = playable.GetInputCount(); // Get all clips on our track

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i); // get inputWeight for our current index

            if (inputWeight > 0f) // Check if inputWeight is above 0, so we know we're working with our active clip
            {
                var inputPlayable = (ScriptPlayable<LightsBehaviour>) playable.GetInput(i); // Use this as our active clip
                var input = inputPlayable.GetBehaviour();

                data.intensity = input.intensity * inputWeight;
                data.range = input.range * inputWeight;

                if (i != previousIndex)
                {
                    data.color = input.color;
                    previousIndex = i;
                }
            }
        }
    }
}