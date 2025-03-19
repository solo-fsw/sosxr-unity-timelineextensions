using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class ExampleMixer : Mixer
    {
        protected override void ProcessingFrame()
        {
        }


        protected override void ActiveBehaviour(Behaviour genericActiveBehaviour, float easeWeight)
        {
            var activeBehaviour = (ExampleBehaviour) genericActiveBehaviour;

            if (activeBehaviour == null)
            {
                Debug.LogWarning("Couldn't cast to correct Behaviour implementation");

                return;
            }

            if (activeBehaviour.Example != null)
            {
                Debug.Log(activeBehaviour.Example.name);
            }
            else
            {
                Debug.Log("No reference, maybe couldn't resolve");
            }

            if (genericActiveBehaviour.ClipHasStartedOnce)
            {
                Debug.Log("Started");
            }

            if (genericActiveBehaviour.EaseInDoneOnce)
            {
                Debug.Log("Ease in finished");
            }

            if (genericActiveBehaviour.EaseOutStartedOnce)
            {
                Debug.Log("Ease out has started");
            }

            if (genericActiveBehaviour.ClipIsDone)
            {
                Debug.Log("Clip done");
            }
        }
    }
}