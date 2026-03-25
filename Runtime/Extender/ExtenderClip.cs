using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     A dummy clip with no playable behaviour. Place the end of this clip slightly after the last real clip in a Timeline
    ///     to give other clips time to complete their end logic before the PlayableGraph is torn down.
    /// </summary>
    [Obsolete("Looper now handles clips at the end of the Timeline without ExtenderClip. Kept for backward compatibility.", false)]
    [Serializable]
    public class ExtenderClip : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.None; // Do not allow blending between clips

        /// <summary>
        ///     We want a null Playable.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) => Playable.Null;
    }
}
