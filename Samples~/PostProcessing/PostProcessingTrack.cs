using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This creates the TrackMixer, and sets the name of the Clip.
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [TrackColor(1, 0, .5f)]
    [TrackClipType(typeof(PostProcessingClip))] // Tell the track that it can create clips from said binding
    public class PostProcessingTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<PostProcessingMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();
            mixer.TrackBinding = TrackBinding;

            return playable;
        }
    }
}