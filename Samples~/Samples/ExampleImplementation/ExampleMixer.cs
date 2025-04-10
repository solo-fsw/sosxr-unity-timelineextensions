using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.TimelineExtensions
{
    public class ExampleMixer : Mixer
    {
        public float ExampleMixerProperty { get; set; }


        protected override void InitializeMixer(Playable playable)
        {
            //
        }


        protected override void ClipStarted(Behaviour activeBehaviour)
        {
            Debug.Log("Started");
        }


        protected override void ClipActive(Behaviour activeBehaviour, float easeWeight)
        {
            var behaviour = (ExampleBehaviour) activeBehaviour;

            if (behaviour == null)
            {
                Debug.LogWarning("Couldn't cast to correct Behaviour implementation");

                return;
            }

            if (behaviour.Example != null)
            {
                Debug.Log(behaviour.Example.name);
            }
            else
            {
                Debug.Log("No reference, maybe couldn't resolve");
            }

            if (activeBehaviour.EaseInDoneOnce)
            {
                Debug.Log("Ease in finished");
            }

            if (activeBehaviour.EaseOutStartedOnce)
            {
                Debug.Log("Ease out has started");
            }
        }


        protected override void ClipEnd(Behaviour activeBehaviour)
        {
            Debug.Log("Clip done");
        }
    }
}