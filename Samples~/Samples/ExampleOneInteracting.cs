using UnityEngine;
using UnityEngine.Timeline;


public class ExampleOneInteracting : MonoBehaviour, ITimeControl
{
    [Tooltip("Put here a gameobject which you want to toggle on or off")]
    [SerializeField] private GameObject gameObjectToToggle;


    public void SetTime(double time)
    {
        // You don't need to use every method in the interface.
    }


    /// <summary>
    ///     For simple code it's usually fine to dump your code directly into the interface method.
    ///     (see also ExampleTwoInteracting)
    /// </summary>
    public void OnControlTimeStart()
    {
        gameObjectToToggle.SetActive(!gameObjectToToggle.activeSelf);
    }


    public void OnControlTimeStop()
    {
        // You don't need to use every method in the interface.
    }
}