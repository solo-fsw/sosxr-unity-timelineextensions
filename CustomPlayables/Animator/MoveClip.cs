using Sirenix.OdinInspector;
using UnityEngine;


public class MoveClip : AnimatorClip
{
	public string xName = "X";
	public string yName = "Y";
	public string zName = "Z";
	public Vector3 movement;
	public bool reset = true;
	[ShowIf(nameof (reset))] public float resetToValue = 0f;


	protected override void SetValuesOnBehaviourFromClip(AnimatorBehaviour behaviour)
	{
		behaviour.xIndex = Animator.StringToHash(xName);
		behaviour.movement.x = movement.x;

		behaviour.yIndex = Animator.StringToHash(yName);
		behaviour.movement.y = movement.y;

		behaviour.zIndex = Animator.StringToHash(zName);
		behaviour.movement.z = movement.z;

		behaviour.reset = reset;
		behaviour.resetToValue = resetToValue;
	}


	protected override string SetDisplayName()
	{
		var dispName = "";
		if (movement.x != 0)
		{
			dispName += xName + colon + movement.x + divider;
		}
		if (movement.y != 0)
		{
			dispName += yName + colon + movement.y + divider;
		}
		if (movement.z != 0)
		{
			dispName += zName + colon + movement.z + divider;
		}
		return dispName;
	}
}
