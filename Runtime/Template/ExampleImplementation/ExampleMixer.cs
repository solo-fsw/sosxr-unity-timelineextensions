using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class ExampleMixer : Mixer<ExampleBehaviour>
    {
        protected override void ActiveBehaviour<T>(T trackBinding, Behaviour genericActiveBehaviour, float easeWeight)
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