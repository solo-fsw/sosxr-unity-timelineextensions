using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the Lights track. Lerps the bound <see cref="Light"/> component's intensity, color, and range between
    ///     their original values and the clip's target values based on the current ease weight.
    ///     When a clip ends without an overlapping clip, it eases out back to the light's original (pre-timeline) values.
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
        ///     The behaviour that started the current ease-out (if any).
        /// </summary>
        private LightsBehaviour _easingOutBehaviour;

        /// <summary>
        ///     Whether we're currently in an ease-out phase, lerping back to original values.
        /// </summary>
        private bool _isEasingOut;

        /// <summary>
        ///     The time (in seconds) remaining in the ease-out phase.
        /// </summary>
        private float _easeOutTimeRemaining;

        /// <summary>
        ///     The light values at the start of ease-out, to lerp from.
        /// </summary>
        private float _easeOutStartIntensity;
        private Color _easeOutStartColor;
        private float _easeOutStartRange;

        protected override void InitializeMixer(Playable playable)
        {
            Binding = (Light)TrackBinding;

            if (Binding == null)
            {
                Debug.LogError(
                    $"{GetType().Name}: There is nothing bound to this Track. Did you forget to set it??"
                );

                return;
            }

            // Capture the light's original values once - these are the pre-timeline values we return to on ease-out
            _originalIntensity = Binding.intensity;
            _originalColor = Binding.color;
            _originalRange = Binding.range;
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (Binding == null)
            {
                return;
            }

            var behaviour = activeBehaviour as LightsBehaviour;

            if (behaviour == null)
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
            if (Binding == null)
            {
                return;
            }

            var behaviour = activeBehaviour as LightsBehaviour;

            if (behaviour == null)
            {
                return;
            }

            // Only apply values from the currently active (driving) behaviour
            if (behaviour != _activeBehaviour)
            {
                return;
            }

            if (_isEasingOut && behaviour == _easingOutBehaviour)
            {
                ApplyEaseOut();
                return;
            }

            // Normal blend: ease-in and hold
            Binding.intensity = Mathf.Lerp(_clipStartIntensity, behaviour.Intensity, easeWeight);
            Binding.color = Color.Lerp(_clipStartColor, behaviour.Color, easeWeight);
            Binding.range = Mathf.Lerp(_clipStartRange, behaviour.Range, easeWeight);
        }

        protected override void ClipEaseOutStartedOnce(Behaviour activeBehaviour)
        {
            if (Binding == null)
            {
                return;
            }

            var behaviour = activeBehaviour as LightsBehaviour;

            // Only handle ease-out for the currently active behaviour
            if (behaviour == null || behaviour != _activeBehaviour)
            {
                return;
            }

            // Start the ease-out phase: lerp from current values back to original values
            _isEasingOut = true;
            _easingOutBehaviour = behaviour;
            _easeOutTimeRemaining = behaviour.MixOutDuration;

            // Capture the current values as the starting point for the ease-out lerp
            _easeOutStartIntensity = Binding.intensity;
            _easeOutStartColor = Binding.color;
            _easeOutStartRange = Binding.range;
        }

        /// <summary>
        ///     Applies the ease-out lerp from the start values to the original values.
        ///     Called every frame during the ease-out phase.
        /// </summary>
        private void ApplyEaseOut()
        {
            if (_easeOutTimeRemaining <= 0)
            {
                // Ease-out complete - snap to original values and clear state
                Binding.intensity = _originalIntensity;
                Binding.color = _originalColor;
                Binding.range = _originalRange;

                _isEasingOut = false;
                _easingOutBehaviour = null;

                return;
            }

            float easeOutDuration = _easingOutBehaviour?.MixOutDuration ?? 0f;

            if (easeOutDuration > 0)
            {
                float t = 1f - (_easeOutTimeRemaining / easeOutDuration);
                t = Mathf.Clamp01(t);

                Binding.intensity = Mathf.Lerp(_easeOutStartIntensity, _originalIntensity, t);
                Binding.color = Color.Lerp(_easeOutStartColor, _originalColor, t);
                Binding.range = Mathf.Lerp(_easeOutStartRange, _originalRange, t);
            }
        }

        protected override void ClipEnd(Behaviour activeBehaviour) { }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            if (_isEasingOut && _easeOutTimeRemaining > 0)
            {
                _easeOutTimeRemaining -= (float)info.deltaTime;
            }
        }
    }
}
