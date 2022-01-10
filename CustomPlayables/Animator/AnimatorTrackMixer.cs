using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;


/// <summary>
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
public class AnimatorTrackMixer : PlayableBehaviour
{
	private AnimatorBehaviour input;
	private int previousIndex = -1;
	private bool priorBool;

	/// <summary>
	///     Tell playable what to do when the playhead is on this clip
	/// </summary>
	/// <param name="playable"></param>
	/// <param name="info"></param>
	/// <param name="playerData"></param>
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		var data = (Animator) playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

		if (!data)
		{
			return;
		}

		if (Application.isPlaying)
		{
			SetValuesToCurrentClipValue(playable, data);
		}
	}


	private void SetValuesToCurrentClipValue(Playable playable, Animator data)
	{
		var inputCount = playable.GetInputCount(); // Get all clips on our track

		for (var i = 0; i < inputCount; i++)
		{
			var inputWeight = playable.GetInputWeight(i); // get inputWeight for our current index

			if (inputWeight > 0f) // Check if inputWeight is above 0, so we know we're working with our active clip
			{
				var inputPlayable = (ScriptPlayable<AnimatorBehaviour>)playable.GetInput(i); // Use this as our active clip
				input = inputPlayable.GetBehaviour();

				DoMove(data, input, inputWeight);

				DoFloat(data, input, inputWeight);

				DoInt(data, input, inputWeight);

				DoTrigger(data, input, i);

				DoBool(data, input, i);

				return;
			}
		}

		ResetFloatNoClipsPlaying(data);
		ResetBool(data);
	}


	private void ResetFloatNoClipsPlaying(Animator data)
	{
		if (input == null)
		{
			return;
		}

		if (input.reset == true)
		{
			if (input.xIndex != 0)
			{
				data.SetFloat(input.xIndex, input.resetToValue);
			}

			if (input.yIndex != 0)
			{
				data.SetFloat(input.yIndex, input.resetToValue);
			}

			if (input.zIndex != 0)
			{
				data.SetFloat(input.zIndex, input.resetToValue);
			}

			if (input.floatIndex != 0)
			{
				data.SetFloat(input.floatIndex, input.resetToValue);
			}
		}
	}


	private static void DoMove(Animator data, AnimatorBehaviour input, float inputWeight)
	{
		if (input.xIndex != 0 && input.movement.x != 0f)
		{
			data.SetFloat(input.xIndex, input.movement.x * inputWeight);
		}

		if (input.yIndex != 0 && input.movement.y != 0f)
		{
			data.SetFloat(input.yIndex, input.movement.y * inputWeight);
		}

		if (input.zIndex != 0 && input.movement.z != 0f)
		{
			data.SetFloat(input.zIndex, input.movement.z * inputWeight);
		}
	}


	private static void DoFloat(Animator data, AnimatorBehaviour input, float inputWeight)
	{
		if (input.floatIndex != 0 && input.floatValue != 0f)
		{
			data.SetFloat(input.floatIndex, input.floatValue * inputWeight);
		}
	}


	private static void DoInt(Animator data, AnimatorBehaviour input, float inputWeight)
	{
		if (input.integerIndex != 0 && input.integerValue != 0)
		{
			data.SetInteger(input.integerIndex, Mathf.RoundToInt(input.integerValue * inputWeight)); // Test the rounding in your project! If problems with rounding, replace with data.SetInteger(input.integerIndex, input.integerValue);
		}
	}


	private void DoTrigger(Animator data, AnimatorBehaviour input, int i)
	{
		if (input.triggerIndex != 0)
		{
			if (input.triggerOnce == true)
			{
				if (i != previousIndex)
				{
					data.SetTrigger(input.triggerIndex);
					GetNextClipInfo(data, input);
					previousIndex = i;
				}
			}
			else if (input.triggerOnce == false)
			{
				data.SetTrigger(input.triggerIndex);
			}
		}
	}


	private static async Task GetNextClipInfo(Animator data, AnimatorBehaviour input)
	{
		var end = Time.time + 1f;

		while (Time.time < end)
		{
			await Task.Yield();
		}

		var clip = data.GetCurrentAnimatorClipInfo(0);

		var triggerClip = (TriggerClip)input.animatorClip;
		triggerClip.animationDuration = clip[0].clip.length;
		triggerClip.animationClipName = clip[0].clip.name;
	}


	private void DoBool(Animator data, AnimatorBehaviour input, int i)
	{
		if (input.boolIndex == 0)
		{
			return;
		}

		if (i == previousIndex)
		{
			return;
		}

		if (input.resetBool == true)
		{
			priorBool = data.GetBool(input.boolIndex);
		}

		data.SetBool(input.boolIndex, input.boolValue);
		previousIndex = i;
	}


	private void ResetBool(Animator data)
	{
		if (input == null)
		{
			return;
		}

		if (input.boolIndex == 0)
		{
			return;
		}

		if (input.resetBool == false)
		{
			return;
		}

		data.SetBool(input.boolIndex, priorBool);
	}
}
