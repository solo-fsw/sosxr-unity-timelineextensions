using TMPro;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the TextMeshPro track. Each frame it finds the active clip, sets the TMP component's text and color
    ///     (alpha driven by ease weight), and only updates the text string when the active clip index changes to avoid
    ///     unnecessary re-layout. Based on <a href="https://youtu.be/12bfRIvqLW4">GameDevGuide</a>.
    /// </summary>
    public class TMProMixer : PlayableBehaviour
    {
        private int _previousIndex = -1;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) // Tell playable what to do when the playhead is on this clip
        {
            TextMeshProUGUI data = (TextMeshProUGUI)playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

            if (!data)
            {
                return;
            }

            int inputCount = playable.GetInputCount(); // Get all clips on our track

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i); // Inputweight for our current index

                if (inputWeight > 0f) // Check if inputWeight is above 0, so we know we're working with our active clip
                {
                    ScriptPlayable<TMProBehaviour> inputPlayable = (ScriptPlayable<TMProBehaviour>)playable.GetInput(i); // Use this as our active clip
                    var input = inputPlayable.GetBehaviour();

                    input.TextColor.a = inputWeight; // Set alpha to the weight of the clip, which allows fading in and out using the ease settings on our clip
                    data.color = input.TextColor;

                    if (i != _previousIndex)
                    {
                        data.text = input.Text;
                        _previousIndex = i;
                    }

                    return;
                }
            }
        }
    }
}
