using System;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    [TrackClipType(typeof(TLActivateClip))]
    [Serializable] // Tell the track that it can create clips from this binding
    public class TLActivateTrack : TrackAsset
    {
    }
}