using System;
using UnityEngine;
using UnityEngine.Playables;


/// <summary>
///     Acts as our data for the clip to write to
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable] public class ParentingBehaviour : PlayableBehaviour
{
	public GameObject trackBinding;
	public GameObject parentToObject;
	public bool zeroInOnParent;
	public Vector3 localPositionOffset;
	public Vector3 localRotationOffset;

	private bool behaviourDone;


	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		var data = (GameObject)playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

		if (!data)
		{
			return;
		}

		if (trackBinding == null)
		{
			trackBinding = data;
		}

		if (!Application.isPlaying)
		{
			return;
		}

		if (behaviourDone == true)
		{
			return;
		}

		if (parentToObject == null)
		{
			trackBinding.transform.parent = null;
			behaviourDone = true;
			return;
		}

		if (trackBinding.transform.parent != parentToObject.transform)
		{
			trackBinding.transform.parent = parentToObject.transform;
		}

		if (zeroInOnParent == true)
		{
			trackBinding.transform.localPosition = Vector3.zero;
			trackBinding.transform.localRotation = new Quaternion();
		}

		if (localPositionOffset != Vector3.zero)
		{
			trackBinding.transform.localPosition = localPositionOffset;
		}

		if (localRotationOffset != Vector3.zero)
		{
			trackBinding.transform.localRotation = Quaternion.Euler(localRotationOffset);
		}

		behaviourDone = true;
	}
}


