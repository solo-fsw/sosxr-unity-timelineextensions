using UnityEngine;
using UnityEngine.Timeline;


public class ExampleTwoInteracting : MonoBehaviour, ITimeControl
{
    [Tooltip("Put here a gameobject with a light attached to it")]
    [SerializeField] private GameObject lights;
    [Tooltip("Put here an AudioSource, which has a clip linked to it, and is set to PlayOnAwake = false")]
    [SerializeField] private AudioSource source;


    public void SetTime(double time)
    {
    }


    /// <summary>
    ///     This is to show a neater way: link your own method inside of the interface method.
    ///     (see also ExampleOneInteracting)
    /// </summary>
    public void OnControlTimeStart()
    {
        ToggleLights();
    }


    /// <summary>
    ///     Easier for repeating code to be put into a method.
    /// </summary>
    public void OnControlTimeStop()
    {
        ToggleLights();
    }


    private void ToggleLights()
    {
        if (lights == null)
        {
            Debug.LogWarning("No lights set");

            return;
        }

        if (source == null)
        {
            Debug.LogWarning("No source set");

            return;
        }

        if (lights.activeSelf == false)
        {
            lights?.SetActive(true);
            source?.Play();
        }
        else if (lights.activeSelf)
        {
            lights?.SetActive(false);
            source?.Play();
        }
    }
}