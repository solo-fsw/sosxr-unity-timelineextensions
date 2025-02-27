using UnityEngine;
using UnityEngine.Playables;


public class TimelineResumer : MonoBehaviour
{
    public bool OperationBufferingAllowed;
    private PlayableDirector _director;
    private bool _haveControl; // To make sure that only the instance called by each clip has control to start the line again.

    public bool Buffered { get; set; }
    public bool IsValid { get; private set; }


    public void OnValidate()
    {
        IsValid = true;
    }


    public void Init(PlayableDirector dir)
    {
        _director = dir;
    }


    /// <summary>
    ///     Call this from the XR Interactable for instance, so that the call can be stored for later, and you don't need
    ///     to perform the action twice in order for the timeline to continue.
    /// </summary>
    public void BufferOperation()
    {
        if (OperationBufferingAllowed)
        {
            Buffered = true;
        }
    }


    public void TakeControl()
    {
        _haveControl = true;
    }


    public void ResumeTimeline()
    {
        if (_director == null)
        {
            Debug.LogWarning("No director was present, cannot Resume");

            return;
        }

        if (_haveControl == false)
        {
            return;
        }

        ExecutiveProducer.SetTimelineSpeed(_director, 1);
        _haveControl = false;
    }
}