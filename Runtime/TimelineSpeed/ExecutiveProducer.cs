using UnityEngine;
using UnityEngine.Playables;


public static class ExecutiveProducer
{
    public static void SetTimelineSpeed(PlayableDirector director, double speed)
    {
        if (director == null)
        {
            Debug.LogWarning("Director is null, yet you're still trying to set the TimelineSpeed. What happened?");

            return;
        }

        if (director.playableGraph.IsValid())
        {
            director.playableGraph.GetRootPlayable(0).SetSpeed(speed);
        }
        else
        {
            Debug.LogWarning("Playable Graph is not valid, yet you're trying to access it! Is this on the last clip in the Timeline by any chance?");
        }
    }
}