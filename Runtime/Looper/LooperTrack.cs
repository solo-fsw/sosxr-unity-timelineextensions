using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.7366781f, 0.3261246f, 0.8529412f)]
    [TrackClipType(typeof(LooperClip))]
    [TrackBindingType(typeof(LooperControl))]
    public class LooperTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var director = graph.GetResolver() as PlayableDirector;
            var playable = ScriptPlayable<LooperMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();
            mixer.Director = director;
            mixer.TrackBinding = TrackBinding;

            return playable;
        }
    }
}