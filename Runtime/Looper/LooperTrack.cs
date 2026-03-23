using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track that binds to a <see cref="LooperControl"/> MonoBehaviour and creates <see cref="LooperClip"/> clips.
    ///     Each clip defines a playback state (loop, pause, jump) that the <see cref="LooperMixer"/> enforces at runtime.
    /// </summary>
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
