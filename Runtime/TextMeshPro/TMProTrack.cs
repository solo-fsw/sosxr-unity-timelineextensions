using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Timeline track that binds to a <see cref="TextMeshProUGUI"/> and creates <see cref="TMProClip"/> clips.
    ///     Drives text content and color (including fade via ease) per clip. Based on <a href="https://youtu.be/12bfRIvqLW4">GameDevGuide</a>.
    /// </summary>
    [TrackColor(0.2f, 0.6f, 0.9f)]
    [TrackBindingType(typeof(TextMeshProUGUI))]
    [TrackClipType(typeof(TMProClip))]
    public class TMProTrack : Track
    {
        protected override Playable CreateMixer(PlayableGraph graph, int inputCount)
        {
            SetDisplayName();

            var playable = ScriptPlayable<TMProMixer>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();

            if (mixer != null)
            {
                mixer.TrackBinding = TrackBinding;
            }

            return playable;
        }

        /// <summary>
        ///     Amended from: https://forum.unity.com/threads/change-clip-name-with-custom-playable.499311/
        /// </summary>
        private void SetDisplayName()
        {
            foreach (var clip in m_Clips)
            {
                TMProClip currentClip = (TMProClip)clip.asset;
                clip.displayName = currentClip.Text + " (" + GetColorInt(currentClip.TextColor.r) + "," + GetColorInt(currentClip.TextColor.g) + "," + GetColorInt(currentClip.TextColor.b) + ")";
            }
        }

        private string GetColorInt(float colorValue)
        {
            string colorInt = Mathf.RoundToInt(colorValue * 255).ToString();

            return colorInt;
        }
    }
}
