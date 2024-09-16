using UnityEngine;
using UnityEngine.Playables;


/// <summary>
/// This acts as our data for the clip to write to
/// Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
public class PostProcessingBehaviour : PlayableBehaviour
{
	public Texture LUTTexture;
	public float contribution;
}
