using System;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     Allows us to set the values in the editor
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable] public class RigidbodyClip : PlayableAsset
{
	public bool isKinematic;
	public bool useGravity;
	public bool addForce;
	public ExposedReference<Transform> target;
	public float amount;
	public ForceMode forceMode;

	private Transform Target
	{
		get => target.Resolve(playableGraph.GetResolver());
	}

	private PlayableGraph playableGraph;

	private TimelineClip timelineClip;

	public TimelineClip TimelineClip
	{
		get => timelineClip;
		set => timelineClip = value;
	}

	private RigidbodyBehaviour template = new RigidbodyBehaviour();

	public RigidbodyBehaviour Template
	{
		get => template;
	}



	/// <summary>
	///     Here we write our logic for creating the playable behaviour
	/// </summary>
	/// <param name="graph"></param>
	/// <param name="owner"></param>
	/// <returns></returns>
	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		var playable = ScriptPlayable<RigidbodyBehaviour>.Create(graph, Template); // Create a playable, using the constructor

		var behaviour = playable.GetBehaviour(); // Get behaviour

		playableGraph = graph;

		SetValuesOnBehaviourFromClip(behaviour);
		SetDisplayName(TimelineClip);

		return playable;
	}


	private void SetValuesOnBehaviourFromClip(RigidbodyBehaviour behaviour)
	{
		behaviour.isKinematic = isKinematic;
		behaviour.useGravity = useGravity;

		if (Target == null)
		{
			return;
		}

		behaviour.target = Target;
		behaviour.addForce = addForce;
		behaviour.amount = amount;
		behaviour.forceMode = forceMode;
	}


	/// <summary>
	///     The displayname of the clip in Timeline will be set using this method.
	///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
	/// </summary>
	private void SetDisplayName(TimelineClip clip)
	{
		if (clip == null)
		{
			return;
		}

		clip.displayName = "";

		if (isKinematic == true)
		{
			clip.displayName += "Kinematic" + " & ";
		}

		if (useGravity == true)
		{
			clip.displayName += "Gravity" + " & ";
		}

		if (addForce == true)
		{
			clip.displayName += "Force: [" + amount + "]" + " (" + Target.name + ")";
		}

		if (clip.displayName.EndsWith(" & "))
		{
			clip.displayName = clip.displayName.Remove(clip.displayName.Length - 3);
		}
	}
}
