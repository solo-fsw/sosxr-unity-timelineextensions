using System;
using UnityEngine;
using UnityEngine.Playables;


/// <summary>
/// This acts as our data for the clip to write to
/// Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable]
public class AnimatorBehaviour : PlayableBehaviour
{
	public AnimatorClip animatorClip;
	public Animator trackBinding;

	public int xIndex;
	public int yIndex;
	public int zIndex;
	public Vector3 movement;

	public string floatName = "";
	public int floatIndex;
	public float floatValue;

	public bool reset;
	public float resetToValue;

	public int integerIndex;
	public int integerValue;

	public string triggerName = "";
	public int triggerIndex;
	public bool triggerOnce = true;
	public bool forceTriggerClipLength = true;

	public string boolName = "";
	public int boolIndex;
	public bool boolValue = true;
	public bool resetBool = true;
}
