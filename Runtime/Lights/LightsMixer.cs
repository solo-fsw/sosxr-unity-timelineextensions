using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Lights track. Lerps the bound <see cref="Light"/> component's intensity, color, and range between
    ///     their starting values and the clip's target values based on the current ease weight.
    ///     When a clip eases out without an overlapping clip, the light lerps back to its original (pre-timeline) values,
    ///     driven by the same ease weight. When that clip ends, the light snaps to the original values.
    /// </summary>
    public class LightsMixer : Mixer
    {
        public Light Binding;

        /// <summary>
        ///     The light's original (pre-timeline) values. Set once in <see cref="InitializeMixer"/> and never modified.
        /// </summary>
        private float _originalIntensity;
        private Color _originalColor;
        private float _originalRange;
        private bool _hasOriginalValues;

        /// <summary>
        ///     Per-clip starting values. Captured when each clip starts, used for ease-in blending.
        /// </summary>
        private float _clipStartIntensity;
        private Color _clipStartColor;
        private float _clipStartRange;

        /// <summary>
        ///     Tracks the currently driving behaviour to handle overlapping clips correctly.
        /// </summary>
        private LightsBehaviour _activeBehaviour;

        /// <summary>
        ///     Whether the active behaviour is in its ease-out phase, lerping back to original values.
        /// </summary>
        private bool _isEasingOut;

        /// <summary>
        ///     The light values at the start of ease-out, to lerp from.
        /// </summary>
        private float _easeOutStartIntensity;
        private Color _easeOutStartColor;
        private float _easeOutStartRange;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = TrackBinding as Light;

            if (Binding == null)
            {
                Debug.LogError($"{TypeName}: There is nothing bound to this Track. Did you forget to set it??");

                return;
            }

            if (_hasOriginalValues)
            {
                return; // OnGraphStart runs again after every Pause / Play. Keep the true pre-timeline values.
            }

            // Capture the light's original values once - these are the pre-timeline values we return to on ease-out
            _originalIntensity = Binding.intensity;
            _originalColor = Binding.color;
            _originalRange = Binding.range;
            _hasOriginalValues = true;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (Binding == null || activeBehaviour is not LightsBehaviour behaviour)
            {
                return;
            }

            // Capture current light values as the starting point for this clip's blend
            _clipStartIntensity = Binding.intensity;
            _clipStartColor = Binding.color;
            _clipStartRange = Binding.range;

            // This clip is now driving the light
            _activeBehaviour = behaviour;
            _isEasingOut = false;
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (Binding == null || activeBehaviour is not LightsBehaviour behaviour)
            {
                return;
            }

            // Only apply values from the currently active (driving) behaviour
            if (behaviour != _activeBehaviour)
            {
                return;
            }

            if (_isEasingOut)
            {
                // easeWeight runs from 1 to 0 during ease-out: 1 = values at ease-out start, 0 = original values.
                Binding.intensity = Mathf.Lerp(_originalIntensity, _easeOutStartIntensity, easeWeight);
                Binding.color = Color.Lerp(_originalColor, _easeOutStartColor, easeWeight);
                Binding.range = Mathf.Lerp(_originalRange, _easeOutStartRange, easeWeight);

                return;
            }

            // Normal blend: ease-in and hold
            Binding.intensity = Mathf.Lerp(_clipStartIntensity, behaviour.Intensity, easeWeight);
            Binding.color = Color.Lerp(_clipStartColor, behaviour.Color, easeWeight);
            Binding.range = Mathf.Lerp(_clipStartRange, behaviour.Range, easeWeight);
        }

        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
            if (Binding == null || activeBehaviour is not LightsBehaviour behaviour)
            {
                return;
            }

            // Only handle ease-out for the currently active behaviour
            if (behaviour != _activeBehaviour)
            {
                return;
            }

            // An overlapping clip takes over through its own ClipStarted / ease-in. Don't fight it.
            if (behaviour.AnotherClipOverlapsWithMe)
            {
                return;
            }

            _isEasingOut = true;

            // Capture the current values as the starting point for the ease-out lerp
            _easeOutStartIntensity = Binding.intensity;
            _easeOutStartColor = Binding.color;
            _easeOutStartRange = Binding.range;
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (Binding == null || activeBehaviour != _activeBehaviour)
            {
                return;
            }

            _activeBehaviour = null;
            _isEasingOut = false;

            if (activeBehaviour is LightsBehaviour { AnotherClipOverlapsWithMe: true })
            {
                return;
            }

            // Ease-out finished (or there was none): make sure the light is fully back at its original values.
            Binding.intensity = _originalIntensity;
            Binding.color = _originalColor;
            Binding.range = _originalRange;
        }
    }
}
