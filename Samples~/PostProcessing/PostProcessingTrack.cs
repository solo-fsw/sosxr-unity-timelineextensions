using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Post Processing track with per-clip Volume references for blending multiple profiles.
    /// </summary>
    [TrackColor(1, 0, .5f)]
    [TrackClipType(typeof(PostProcessingClip))]
    public class PostProcessingTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            var playable = ScriptPlayable<PostProcessingMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();

            return playable;
        }
    }
}
