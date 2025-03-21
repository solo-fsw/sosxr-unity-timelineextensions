using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.7366781f, 0.3261246f, 0.8529412f)]
    [TrackClipType(typeof(TimeControlClip))]
    [TrackBindingType(typeof(TimelineControl))]
    public class TimeControlTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var director = graph.GetResolver() as PlayableDirector;
            var playable = ScriptPlayable<TimeControlMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();
            mixer.Director = director;

            return playable;
        }
    }
}