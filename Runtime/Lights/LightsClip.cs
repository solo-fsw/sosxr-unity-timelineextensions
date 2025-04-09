using System;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class LightsClip : Clip
    {
        [NoFoldOut] public LightsBehaviour Template;


        /// <summary>
        ///     Here we write our logic for creating the playable behaviour
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var light = TrackBinding as Light;

            if (light != null && !Application.isPlaying)
            {
                Template.OriginalIntensity = light.intensity;
                Template.OriginalColor = light.color;
                Template.OriginalRange = light.range;
            }

            var playable = ScriptPlayable<LightsBehaviour>.Create(graph, Template); // Create a playable using the constructor

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
            var r = Math.Round(Template.Color.r, 3);
            var g = Math.Round(Template.Color.g, 3);
            var b = Math.Round(Template.Color.b, 3);
            TimelineClip.displayName = "I:" + Template.Intensity + " R:" + Template.Range + " (" + r + "," + g + "," + b + ")";
        }
    }
}