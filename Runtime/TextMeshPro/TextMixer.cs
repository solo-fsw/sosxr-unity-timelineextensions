using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mixer for the TextMeshPro track. Each frame it finds the active clip, sets the TMP component's text and color
    ///     (alpha driven by ease weight), and only updates the text string when the active clip index changes to avoid
    ///     unnecessary re-layout. Based on <a href="https://youtu.be/12bfRIvqLW4">GameDevGuide</a>.
    /// </summary>
    public class TextMixer : Mixer
    {
        private readonly List<TextBehaviour> _behaviours = new();
        private readonly Dictionary<TextBehaviour, int> _behaviourToIndex = new();
        private int _previousIndex = -1;
        private TextMeshProUGUI _binding;
        private string _startText;
        private Color _startColor;


        protected override void InitializeMixer(Playable playable)
        {
            _binding = TrackBinding as TextMeshProUGUI;

            if (_binding == null)
            {
               return;
            }

            _startText = _binding?.text;
            _startColor = _binding.color;

            _behaviours.Clear();
            _behaviourToIndex.Clear();

            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                ScriptPlayable<TextBehaviour> inputPlayable = (ScriptPlayable<TextBehaviour>)playable.GetInput(i);
                var behaviour = inputPlayable.GetBehaviour();
                _behaviours.Add(behaviour);
                _behaviourToIndex[behaviour] = i;
            }
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
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

            var behaviour = activeBehaviour as TextBehaviour;

            if (behaviour == null)
            {
                return;
            }

            int currentIndex = _behaviourToIndex.TryGetValue(behaviour, out var index) ? index : -1;

            // if (currentIndex != _previousIndex)
            {
                _binding.text = behaviour.Text;
                _previousIndex = currentIndex;
            }

            Color textColor = behaviour.TextColor;
            textColor.a = easeWeight;
            _binding.color = textColor;
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (_binding == null)
            {
                return;
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
