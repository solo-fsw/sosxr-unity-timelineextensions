using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    [TrackColor(0.468f, 0.704f, 0.818f)]
    [TrackBindingType(typeof(Light))] // Bind to whatever you need to control in Timeline
    [TrackClipType(typeof(LightsClip))] // Tell the track that it can create clips from said binding
    /// <summary>
    ///     Timeline track that binds to a <see cref="Light"/> component and creates <see cref="LightsClip"/> clips.
    ///     Use it to animate a light's intensity, color, and range from Timeline with easing support.
    ///     The light must be set to Mixed or Realtime mode.
    /// </summary>
    public class LightsTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            ScriptPlayable<LightsMixer> playable = ScriptPlayable<LightsMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();
            mixer.TrackBinding = TrackBinding;

            return playable;
        }
    }
}
