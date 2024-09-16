using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


/// <summary>
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
public class PostProcessingTrackMixer : PlayableBehaviour
{
    private Texture currentLUTTexture;
    private float currentContribution;
    private ColorLookup colorLookup;

    private bool valueHasBeenSet;
    private int previousIndex = -1;


    /// <summary>
    ///     Tell playable what to do when the playhead is on this clip
    /// </summary>
    /// <param name="playable"></param>
    /// <param name="info"></param>
    /// <param name="playerData"></param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var data = (VolumeProfile) playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

        if (!data)
        {
            return;
        }

        if (colorLookup == null)
        {
            data.TryGet(out colorLookup);
        }

        if (Application.isPlaying)
        {
            GetCurrentClipValue(playable);
        }
    }


    private void GetCurrentClipValue(Playable playable)
    {
        var inputCount = playable.GetInputCount(); // Get all clips on our track

        for (var i = 0; i < inputCount; i++)
        {
            var inputWeight = playable.GetInputWeight(i); // get inputWeight for our current index

            if (inputWeight > 0f) // Check if inputWeight is above 0, so we know we're working with our active clip
            {
                var inputPlayable = (ScriptPlayable<PostProcessingBehaviour>) playable.GetInput(i); // Use this as our active clip
                var input = inputPlayable.GetBehaviour();

                if (i != previousIndex)
                {
                    colorLookup.texture.value = input.LUTTexture;
                    previousIndex = i;
                }

                colorLookup.contribution.value = input.contribution * inputWeight; // Multiply values by inputWeight to allow easing in and out

                return;
            }
        }
    }
}