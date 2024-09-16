using System;
using UnityEngine;
using UnityEngine.Playables;


/// <summary>
///     Acts as our data for the clip to write to
///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
/// </summary>
[Serializable]
public class InteractBehaviour : PlayableBehaviour
{
    public IInteract interact;
    private bool interacted;


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var data = (GameObject) playerData; // The playerData is the object that our track is bound to, so cast to the binding of the Track

        if (data == null)
        {
            return;
        }

        if (interact == null && data.GetComponent<IInteract>() != null)
        {
            interact = data.GetComponent<IInteract>();
        }

        if (!Application.isPlaying)
        {
            return;
        }

        if (interacted == false)
        {
            interact?.Interact();
            interacted = true;
        }
    }
}