using System;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Clip asset for the Lights track. Captures the light's original intensity, color, and range at edit time so the
    ///     mixer can blend back to them during ease-out. The clip display name shows the configured values at a glance.
    /// </summary>
    [Serializable]
    public class LightsClip : Clip
    {
        [NoFoldOut]
        public LightsBehaviour Template;

        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Light light = TrackBinding as Light;

            if (light != null && !Application.isPlaying)
            {
                Template.OriginalIntensity = light.intensity;
                Template.OriginalColor = light.color;
                Template.OriginalRange = light.range;
            }

            ScriptPlayable<LightsBehaviour> playable = ScriptPlayable<LightsBehaviour>.Create(
                graph,
                Template
            ); // Create a playable using the constructor

            var behaviour = playable.GetBehaviour(); // Get behaviour

            behaviour.InitializeBehaviour(TimelineClip, TrackBinding);

            behaviour.Intensity = Template.Intensity;
            behaviour.Color = Template.Color;
            behaviour.Range = Template.Range;

            SetDisplayName();

            return playable;
        }

        private void SetDisplayName()
        {
            double r = Math.Round(Template.Color.r, 3);
            double g = Math.Round(Template.Color.g, 3);
            double b = Math.Round(Template.Color.b, 3);

            TimelineClip.displayName =
                "I:"
                + Template.Intensity
                + " R:"
                + Template.Range
                + " ("
                + r
                + ","
                + g
                + ","
                + b
                + ")";
        }
    }
}
