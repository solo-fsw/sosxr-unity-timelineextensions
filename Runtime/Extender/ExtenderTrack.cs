using System;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     A no-binding track that only contains <see cref="ExtenderClip"/> clips.
    ///     Add this track and extend its clip beyond all other clips to ensure the PlayableGraph remains valid long enough
    ///     for every other clip to finish its end logic.
    /// </summary>
    [Obsolete("Looper now handles clips at the end of the Timeline without ExtenderTrack. Kept for backward compatibility.", false)]
    [TrackColor(0.0f, 0.17f, 0.88f)] // A dark blue, Leiden University's house colour
    [TrackClipType(typeof(ExtenderClip))]
    public class ExtenderTrack : TrackAsset
    {
    }
}
