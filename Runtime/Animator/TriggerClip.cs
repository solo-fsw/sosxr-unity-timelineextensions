
using UnityEngine;
using UnityEngine.Timeline;


public class TriggerClip : AnimatorClip
{
	public string animationClipName;
	public float animationDuration;

	protected override void SetValuesOnBehaviourFromClip(AnimatorBehaviour behaviour)
	{
		behaviour.animatorClip = this;
	}

	/// <summary>
	/// Here we set the clip duration to the length that's set by the values on the clip itself.
	/// </summary>
	/// <param name="clip"></param>
	protected override void SetClipDuration(TimelineClip clip)
	{
		if (template.forceTriggerClipLength == true && animationDuration != 0)
		{
			clip.duration = animationDuration;
		}
	}


	protected override string SetDisplayName()
	{
		var displayName = "";
		if (template.triggerName != "")
		{
			displayName += template.triggerName;
		}

		return displayName;
	}
}
