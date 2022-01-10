using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
/// These variables allow us to set the value in the editor.
/// Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
public class PostProcessingClip : PlayableAsset
{
	public Texture LUTTexture;
	[Range(0f, 1f)] public float contribution = 1f;

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
		var playable = ScriptPlayable<PostProcessingBehaviour>.Create(graph); // Create a playable using the constructor

		var behaviour = playable.GetBehaviour(); // Get behaviour

		SetValuesOnBehaviourFromClip(behaviour);
		SetDisplayName(TimelineClip);

		return playable;
	}


	private void SetValuesOnBehaviourFromClip(PostProcessingBehaviour behaviour)
	{
		behaviour.LUTTexture = LUTTexture;
		behaviour.contribution = contribution;
	}


	/// <summary>
	/// The displayname of the clip in Timeline will be set using this method.
	/// Name is only set if a varable is used (in case of X/Y/Z if they have a value != 0, in other cases if the string name of the variable is not null).
	/// Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
	/// </summary>
	private void SetDisplayName(TimelineClip clip)
	{
		var displayName = "";

		if (LUTTexture != null)
		{
			displayName += LUTTexture.name + " C:" + contribution + "f";
		}

		displayName = SetDisplayNameIfStillEmpty(displayName);

		if (clip != null)
		{
			clip.displayName = displayName;
		}
	}


	private static string SetDisplayNameIfStillEmpty(string dispName)
	{
		if (string.IsNullOrEmpty(dispName))
		{
			dispName = "New PostProcessing Clip";
		}

		return dispName;
	}

}
