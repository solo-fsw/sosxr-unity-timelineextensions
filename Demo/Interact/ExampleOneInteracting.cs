using UnityEngine;


public class ExampleOneInteracting : MonoBehaviour, IInteract
{
    [Tooltip("Put here a gameobject which you want to toggle on or off")]
    [SerializeField] private GameObject gameobjectToToggle;


    /// <summary>
    ///     For simple code it's usually fine to dump your code directly into the interface method.
    ///     (see also ExampleTwoInteracting)
    /// </summary>
    public void Interact()
    {
        if (gameobjectToToggle.activeSelf == false)
        {
            gameobjectToToggle.SetActive(true);
        }
        else if (gameobjectToToggle.activeSelf)
        {
            gameobjectToToggle.SetActive(false);
        }
    }
}