using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     Allows us to set the values in the editor
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable] public class ParentingClip : PlayableAsset
{
	private PlayableGraph playableGraph;
	public ExposedReference<GameObject> parentToObject; // See: https://docs.unity3d.com/ScriptReference/ExposedReference_1.html
	private GameObject ParentToObject
	{
		get => parentToObject.Resolve(playableGraph.GetResolver());
	}

	[SerializeField] public bool zeroInOnParent;

	[Header("Alternatively: use an additional child of parent object as this object's parent")] [SerializeField]
	public Vector3 localPositionOffset;
	public Vector3 localRotationOffset;

	private TimelineClip timelineClip;
	public TimelineClip TimelineClip
	{
		get => timelineClip;
		set => timelineClip = value;
	}

	private ParentingBehaviour template = new ParentingBehaviour();

	public ParentingBehaviour Template
	{
		get => template;
	}

	private const string divider = " - ";
	private const string defaultDisplayName = "Orphanize";


	/// <summary>
	///     Here we write our logic for creating the playable behaviour
	/// </summary>
	/// <param name="graph"></param>
	/// <param name="owner"></param>
	/// <returns></returns>
	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		var playable = ScriptPlayable<ParentingBehaviour>.Create(graph, template); // Create a playable, using the constructor

		var behaviour = playable.GetBehaviour(); // Get behaviour

		playableGraph = graph;
		SetValuesOnBehaviourFromClip(behaviour);
		SetDisplayName(TimelineClip);

		return playable;
	}


	private void SetValuesOnBehaviourFromClip(ParentingBehaviour behaviour)
	{
		if (ParentToObject == null)
		{
			return;
		}

		behaviour.parentToObject = ParentToObject;
		behaviour.zeroInOnParent = zeroInOnParent;
		behaviour.localPositionOffset = localPositionOffset;
		behaviour.localRotationOffset = localRotationOffset;
	}


	/// <summary>
	///     The displayname of the clip in Timeline will be set using this method.
	///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
	/// </summary>
	private void SetDisplayName(TimelineClip clip)
	{
		var displayName = "";

		if (clip == null)
		{
			return;
		}

		if (ParentToObject != null)
		{
			displayName += ParentToObject.name;
		}

		displayName = RemoveTrailingDivider(displayName);
		displayName = SetDisplayNameIfStillEmpty(displayName);

		clip.displayName = displayName;
	}


	private static string RemoveTrailingDivider(string dispName)
	{
		if (!string.IsNullOrEmpty(dispName))
		{
			var removeLast = dispName.LastIndexOf(divider, StringComparison.Ordinal);

			if (removeLast < 0)
			{
				return dispName;
			}

			dispName = dispName.Remove(removeLast);
			return dispName;
		}

		return dispName;
	}


	private static string SetDisplayNameIfStillEmpty(string dispName)
	{
		if (string.IsNullOrEmpty(dispName))
		{
			dispName = defaultDisplayName;
		}

		return dispName;
	}
}
