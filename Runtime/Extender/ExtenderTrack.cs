using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.468f, 0.704f, 0.818f)]
    [TrackClipType(typeof(ExtenderClip))] // Tell the track that it can create clips from said binding
    public class ExtenderTrack : TrackAsset
    {
    }
}