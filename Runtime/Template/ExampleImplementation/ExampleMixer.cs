using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class ExampleMixer : Mixer<ExampleBehaviour>
    {
        protected override void ActiveBehaviour<T>(T trackBinding, Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour.ClipHasStartedOnce)
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

            if (activeBehaviour.ClipIsDone)
            {
                Debug.Log("Clip done");
            }

            var myActiveBehaviour = (ExampleBehaviour) activeBehaviour;
            
            if (myActiveBehaviour.Example != null)
            {
                Debug.Log(myActiveBehaviour.Example.name);
            }
            else
            {
                Debug.Log("No reference, maybe couldn't resolve");
            }
        }
    }
}