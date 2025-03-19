using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class Mixer : TLMixer
    {
        protected override void ActiveBehaviour<T>(T trackBinding, Behaviour activeBehaviour, float easeWeight)
        {
            if (activeBehaviour.ClipHasStarted)
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