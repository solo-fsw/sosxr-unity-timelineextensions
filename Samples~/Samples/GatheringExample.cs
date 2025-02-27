using UnityEngine;


/// <summary>
///     This is an example of how you can use the ILoopBreaker interface to break out of a loop in Timeline.
/// </summary>
public class GatheringExample : TimeControlBase
{
    public int StuffGathered = 0;
    
    
    
    [ContextMenu(nameof(StartGathering))]
    public void StartGathering()
    {
        Debug.Log("Gathering started!");
        Pause();
    }


    [ContextMenu(nameof(StopGathering))]
    public void StopGathering()
    {
        Debug.Log("Gathering stopped!");
        Continue();
    }
    
    [ContextMenu(nameof(GatherStuff))]
    public void GatherStuff()
    {
        StuffGathered++;
        Debug.Log("Gathered stuff! Total gathered: " + StuffGathered);
    }


    private void Update()
    {
        if (StuffGathered >= 5)
        {
            Debug.Log("Gathering complete!");
            StopGathering();
        }
    }
}