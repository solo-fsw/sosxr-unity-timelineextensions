using SOSXR.TimelineExtensions;
using UnityEngine;
using UnityEngine.Timeline;


public class OtherGatheringExample : TimeControlBase, ITimeControl
{
    public float GatheringDuration = 60f;

    private float _gatheringTimer;

    private bool _isGathering;


    public override void OnControlTimeStart()
    {
        base.OnControlTimeStart();

        _isGathering = true;
    }


    public override void OnControlTimeStop()
    {
        base.OnControlTimeStop();
        _isGathering = false;
    }


    [ContextMenu(nameof(StartGathering))]
    public void StartGathering()
    {
        Debug.Log("Gathering started!");
        TimeScaleZero();
    }


    private void Update()
    {
        if (_isGathering)
        {
            _gatheringTimer = _gatheringTimer + Time.deltaTime;

            if (_gatheringTimer >= GatheringDuration)
            {
                Debug.Log("Gathering complete!");
                StopGathering();
            }
        }
    }


    [ContextMenu(nameof(StopGathering))]
    public void StopGathering()
    {
        Debug.Log("Gathering stopped!");
        BreakAndGoToEnd();
    }
}