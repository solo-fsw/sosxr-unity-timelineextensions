using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Lights track. Lerps the bound <see cref="Light"/> component's intensity, color, and range between
    ///     their original values and the clip's target values based on the current ease weight.
    /// </summary>
    public class LightsMixer : Mixer
    {
        public Light Binding;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = (Light)TrackBinding;
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (Binding == null)
            {
                Debug.LogError($"{GetType().Name}: There is nothing bound to this Track. Did you forget to set it??");

                return;
            }

            LightsBehaviour behaviour = activeBehaviour as LightsBehaviour;

            Binding.intensity = Mathf.Lerp(behaviour.OriginalIntensity, behaviour.Intensity, easeWeight);
            Binding.color = Color.Lerp(behaviour.OriginalColor, behaviour.Color, easeWeight);
            Binding.range = Mathf.Lerp(behaviour.OriginalRange, behaviour.Range, easeWeight);
        }
    }
}
