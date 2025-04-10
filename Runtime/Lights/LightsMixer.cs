using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class LightsMixer : Mixer
    {
        private Light _light;


        protected override void InitializeMixer(Playable playable)
        {
            _light = (Light) TrackBinding;
        }


        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            var behaviour = activeBehaviour as LightsBehaviour;

            _light.intensity = Mathf.Lerp(behaviour.OriginalIntensity, behaviour.Intensity, easeWeight);
            _light.color = Color.Lerp(behaviour.OriginalColor, behaviour.Color, easeWeight);
            _light.range = Mathf.Lerp(behaviour.OriginalRange, behaviour.Range, easeWeight);
        }
    }
}