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
        private float _startIntensity;
        private Color _startColor;
        private float _startRange;
        private LightsBehaviour _behaviour;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = (Light)TrackBinding;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (Binding == null)
            {
                Debug.LogError(
                    $"{GetType().Name}: There is nothing bound to this Track. Did you forget to set it??"
                );

                return;
            }

            _behaviour = activeBehaviour as LightsBehaviour;

            if (_behaviour == null)
            {
                return;
            }

            _startIntensity = Binding.intensity;
            _startColor = Binding.color;
            _startRange = Binding.range;
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (Binding == null || _behaviour == null)
            {
                return;
            }
            Binding.intensity = Mathf.Lerp(_startIntensity, _behaviour.Intensity, easeWeight);

            Binding.color = Color.Lerp(_startColor, _behaviour.Color, easeWeight);
            Binding.range = Mathf.Lerp(_startRange, _behaviour.Range, easeWeight);
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            Debug.Log("No way to go back to baseline yet");
        }
    }
}
