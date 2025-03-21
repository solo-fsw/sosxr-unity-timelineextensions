using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class LightsMixer : Mixer
    {
        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not LightsBehaviour behaviour)
            {
                return;
            }

            var light = (Light) activeBehaviour.TrackBinding;

            light.intensity = Mathf.Lerp(behaviour.OriginalIntensity, behaviour.Intensity, easeWeight);
            light.color = Color.Lerp(behaviour.OriginalColor, behaviour.Color, easeWeight);
            light.range = Mathf.Lerp(behaviour.OriginalRange, behaviour.Range, easeWeight);
        }
    }
}