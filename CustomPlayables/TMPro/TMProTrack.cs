using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
///     From GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[TrackBindingType(typeof (TextMeshProUGUI))] // Bind to whatever I need to have in the Timeline
[TrackClipType(typeof (TMProClip))] // Tell the track that it can create clips from this binding
public class TMProTrack : TrackAsset
{
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) // Tell our track to use the trackMixer to control our playableBehaviours
	{
		SetDisplayName();

		return ScriptPlayable<TMProTrackMixer>.Create(graph, inputCount);
	}


	/// <summary>
	///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
	/// </summary>
	private void SetDisplayName()
	{
		foreach (var clip in m_Clips)
		{
			var currentClip = (TMProClip) clip.asset;
			clip.displayName = currentClip.text + " (" + GetColorInt(currentClip.color.r) + "," + GetColorInt(currentClip.color.g) + "," + GetColorInt(currentClip.color.b) + ")";
		}
	}


	private string GetColorInt(float colorValue)
	{
		var colorInt = Mathf.RoundToInt(colorValue * 255).ToString();
		return colorInt;
	}
}
