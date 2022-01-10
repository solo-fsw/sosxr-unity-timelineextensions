using UnityEngine;


public class ExampleTwoInteracting : MonoBehaviour, IInteract
{
	[Tooltip("Put here a gameobject with a light attached to it")]
	[SerializeField] private GameObject lights;
	[Tooltip("Put here an AudioSource, which has a clip linked to it, and is set to PlayOnAwake = false")]
	[SerializeField] private AudioSource source;


	/// <summary>
	/// This is to show a neater way: link your own method inside of the interface method.
	/// (see also ExampleOneInteracting)
	/// </summary>
	public void Interact()
	{
		ToggleLights();
	}


	private void ToggleLights()
	{
		if (lights.activeSelf == false)
		{
			lights.SetActive(true);
			source.Play();
		}
		else if (lights.activeSelf == true)
		{
			lights.SetActive(false);
			source.Play();
		}
	}
}
