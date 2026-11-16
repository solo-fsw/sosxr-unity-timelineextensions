using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the TextMeshPro track. Each frame it picks the active clip with the highest weight, sets the TMP
    ///     component's text and color (alpha driven by ease weight), and only updates the text string when that clip changes
    ///     to avoid unnecessary re-layout. Based on <a href="https://youtu.be/12bfRIvqLW4">GameDevGuide</a>.
    /// </summary>
    public class TextMixer : Mixer
    {
        private TextMeshProUGUI _binding;
        private string _startText;
        private Color _startColor;
        private bool _hasStartValues;

        /// <summary>The clip whose text is currently on the binding. The text is only set again when this changes.</summary>
        private TextBehaviour _shownBehaviour;

        /// <summary>Highest-weight active clip of the frame being processed.</summary>
        private TextBehaviour _frameBehaviour;
        private float _frameWeight;


        /// <summary>Caches the bound text component. On the first call only, also stores its original text and color for restoring later.</summary>
        protected override void InitializeMixer(Playable playable)
        {
            _binding = TrackBinding as TextMeshProUGUI;

            if (_binding == null)
            {
               return;
            }

            if (_hasStartValues)
            {
                return; // OnGraphStart runs again after every Pause / Play. Keep the true pre-timeline values.
            }

            _startText = _binding.text;
            _startColor = _binding.color;
            _hasStartValues = true;
        }

        /// <summary>Records the active TextBehaviour with the highest ease weight this frame. The result is applied in AllClipsActiveDone.</summary>
        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not TextBehaviour behaviour)
            {
                return;
            }

            // Overlapping clips: the one with the highest weight drives the text (applied in AllClipsActiveDone).
            if (_frameBehaviour == null || easeWeight > _frameWeight)
            {
                _frameBehaviour = behaviour;
                _frameWeight = easeWeight;
            }
        }

        /// <summary>Applies the frame's highest-weight clip to the binding. Text is set only when that clip changes, and alpha follows the ease weight.</summary>
        protected override void AllClipsActiveDone()
        {
            var behaviour = _frameBehaviour;
            var easeWeight = _frameWeight;
            _frameBehaviour = null;

            if (behaviour == null)
            {
                return;
            }

            if (_binding == null)
            {
                _binding = TrackBinding as TextMeshProUGUI;
            }

            if (_binding == null)
            {
                Debug.LogError(
                    $"{TypeName}: There is nothing bound to this Track. Did you forget to set it??"
                );

                return;
            }

            _binding.gameObject.SetActive(true);
            _binding.enabled = true;

            // Only set the text when the driving clip changes: assigning .text makes TMP rebuild its layout.
            if (behaviour != _shownBehaviour)
            {
                _binding.text = behaviour.Text;
                _shownBehaviour = behaviour;
            }

            Color textColor = behaviour.TextColor;
            textColor.a = easeWeight;
            _binding.color = textColor;
        }

        /// <summary>Clears the shown-clip cache. Restores the original text and color once no other TextBehaviour is still active.</summary>
        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not TextBehaviour)
            {
                return;
            }

            // Force the next driving clip to set its text again.
            _shownBehaviour = null;

            if (_binding == null || !_hasStartValues)
            {
                return;
            }

            // Another clip is still active: it takes over next frame, no need to restore in between.
            foreach (var other in Behaviours)
            {
                if (other is TextBehaviour { ClipIsActive: true } && other != activeBehaviour)
                {
                    return;
                }
            }

            _binding.text = _startText;
            _binding.color = _startColor;
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
