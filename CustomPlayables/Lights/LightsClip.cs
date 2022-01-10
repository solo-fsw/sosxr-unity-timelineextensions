using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
/// These variables allow us to set the value in the editor.
/// Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
public class LightsClip : PlayableAsset
{
	[Range(0f, 50f)] public float range = 10f;
	[Range(0f, 50f)] public float intensity = 1f;
	public Color32 color;

	private TimelineClip timelineClip;
	public TimelineClip TimelineClip
	{
		get => timelineClip;
		set => timelineClip = value;
	}


	/// <summary>
	/// Here we write our logic for creating the playable behaviour
	/// </summary>
	/// <param name="graph"></param>
	/// <param name="owner"></param>
	/// <returns></returns>
	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		var playable = ScriptPlayable<LightsBehaviour>.Create(graph); // Create a playable using the constructor

		var behaviour = playable.GetBehaviour(); // Get behaviour

		SetValuesOnBehaviourFromClip(behaviour);
		SetDisplayName();

		return playable;
	}


	private void SetValuesOnBehaviourFromClip(LightsBehaviour behaviour)
	{
		behaviour.intensity = intensity;
		behaviour.range = range;
		behaviour.color = color;
	}


	/// <summary>
	/// The displayname of the clip in Timeline will be set using this method.
	/// Name is only set if a light is set to != 0;
	/// Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
	/// </summary>
	private void SetDisplayName()
	{
		var displayName = "";

		if (intensity != 0)
		{
			displayName += "I:" + intensity + " R:" + range +  " (" + color.r + "," + color.g + "," + color.b + ")";
		}

		displayName = SetDisplayNameIfStillEmpty(displayName);

		if (TimelineClip != null)
		{
			TimelineClip.displayName = displayName;
		}
	}


	private static string SetDisplayNameIfStillEmpty(string dispName)
	{
		if (string.IsNullOrEmpty(dispName))
		{
			dispName = "New Lights Clip";
		}

		return dispName;
	}
}
