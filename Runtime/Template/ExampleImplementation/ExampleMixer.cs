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

            Debug.Log("Clippy");
        }
    }
}