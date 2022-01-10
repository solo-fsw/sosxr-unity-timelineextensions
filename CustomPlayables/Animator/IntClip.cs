using UnityEngine;


public class IntClip : AnimatorClip
{
	public string integerName = "";
	public int integerValue;


	protected override void SetValuesOnBehaviourFromClip(AnimatorBehaviour behaviour)
	{
		behaviour.integerIndex = Animator.StringToHash(integerName);
		behaviour.integerValue = integerValue;
	}


	protected override string SetDisplayName()
	{
		var dispName = "";
		if (integerName != "")
		{
			dispName += integerName + colon + integerValue;
		}
		return dispName;
	}
}
