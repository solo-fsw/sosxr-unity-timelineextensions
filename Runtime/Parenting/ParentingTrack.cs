using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track that binds to a parent <see cref="Transform"/> and creates <see cref="ParentingClip"/> clips.
    ///     For each clip, the specified child Transform is temporarily reparented to the track's bound Transform,
    ///     then restored to its original parent when the clip ends.
    /// </summary>
    [TrackColor(0.468f, 0.704f, 0.818f)]
    [TrackBindingType(typeof(Transform))] // Bind to whatever you need to have in the Timeline
    [TrackClipType(typeof(ParentingClip))] // Tell the track that it can create clips from this binding
    public class ParentingTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            ScriptPlayable<ParentingMixer> playable = ScriptPlayable<ParentingMixer>.Create(
                graph,
                inputCount
            );

            var mixer = playable.GetBehaviour();
            mixer.TrackBinding = TrackBinding;

            return playable;
        }
    }
}
