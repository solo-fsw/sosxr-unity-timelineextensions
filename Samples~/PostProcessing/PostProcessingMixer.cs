using UnityEngine.Playables;
using UnityEngine.Rendering;


namespace SOSXR.TimelineExtensions
{
    public class PostProcessingMixer : Mixer
    {
        private Volume _currentVolume;
        private float _blendStartWeight;
        private bool _inTransition;


        protected override void InitializeMixer(Playable playable)
        {
        }

        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not PostProcessingBehaviour behaviour)
            {
                return;
            }

            if (behaviour.Volume == null)
            {
                return;
            }

            if (_currentVolume == behaviour.Volume && _currentVolume != null)
            {
                _blendStartWeight = _currentVolume.weight;
                _inTransition = true;
            }
            else
            {
                _inTransition = false;
            }

            _currentVolume = behaviour.Volume;
        }

        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour is not PostProcessingBehaviour behaviour)
            {
                return;
            }

            if (behaviour.Volume == null)
            {
                return;
            }

            float targetWeight;

            if (_inTransition && behaviour.Volume == _currentVolume)
            {
                targetWeight = Mathf.Lerp(_blendStartWeight, behaviour.MaxWeight, easeWeight);
            }
            else
            {
                targetWeight = easeWeight * behaviour.MaxWeight;
            }

            behaviour.Volume.weight = targetWeight;
        }

        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            if (activeBehaviour is not PostProcessingBehaviour behaviour)
            {
                return;
            }

            if (behaviour.Volume == null)
            {
                return;
            }

            if (_currentVolume == behaviour.Volume)
            {
                behaviour.Volume.weight = 0f;
                _currentVolume = null;
                _inTransition = false;
            }
        }
    }
}
