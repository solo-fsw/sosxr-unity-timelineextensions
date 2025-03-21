using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class ExampleMixer : Mixer
    {
        public float ExampleMixerProperty { get; set; }


        /// <summary>
        ///     If you need to do something every frame, do it here.
        ///     This is called before the ActiveBehaviour method.
        /// </summary>
        /// <param name="easeWeight"></param>
        /// <param name="inputCount"></param>
        protected override void ProcessingFrame(float easeWeight)
        {
        }


        protected override void ActiveBehaviour(Behaviour activeBehaviour, float easeWeight)
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

            if (activeBehaviour.ClipStartedOnce)
            {
                Debug.Log("Started");
            }

            if (activeBehaviour.EaseInDoneOnce)
            {
                Debug.Log("Ease in finished");
            }

            if (activeBehaviour.EaseOutStartedOnce)
            {
                Debug.Log("Ease out has started");
            }

            if (activeBehaviour.ClipEnd)
            {
                Debug.Log("Clip done");
            }
        }
    }
}