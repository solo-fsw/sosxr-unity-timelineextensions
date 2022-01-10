using TMPro;
using UnityEngine.Playables;


/// <summary>
/// From GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
public class TMProTrackMixer : PlayableBehaviour
{
	private int previousIndex = -1;

	public override void ProcessFrame(Playable playable, FrameData info, object playerData) // Tell playable what to do when the playhead is on this clip
	{
		var data = (TextMeshProUGUI) playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

		if (!data)
		{
			return;
		}

		int inputCount = playable.GetInputCount(); // Get all clips on our track

		for (int i = 0; i < inputCount; i++)
		{
			float inputWeight = playable.GetInputWeight(i);  // Inputweight for our current index

			if (inputWeight > 0f) // Check if inputWeight is above 0, so we know we're working with our active clip
			{
				var inputPlayable = (ScriptPlayable<TMProBehaviour>)playable.GetInput(i); // Use this as our active clip
				var input = inputPlayable.GetBehaviour();

				input.color.a = inputWeight; // Set alpha to the weight of the clip, which allows fading in and out using the ease settings on our clip
				data.color = input.color;

				if (i != previousIndex)
				{
					data.text = input.text;
					previousIndex = i;
				}

				return;
			}
		}
	}
}
