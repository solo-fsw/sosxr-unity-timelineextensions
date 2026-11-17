using UnityEngine;

namespace SOSXR.TimelineExtensions.Samples
{
    /// <summary>
    ///     Sample implementation of <see cref="IInterface"/> that logs each lifecycle callback to the console.
    ///     Attach this to a GameObject and bind it to an <see cref="InterfaceTrack"/> to see the callback sequence.
    /// </summary>
    public class InterfaceExample : MonoBehaviour, IInterface
    {
        public float Weight;

        public void OnClipStart(float easeInDuration)
        {
            Debug.LogFormat(this, "OnClipStart");
        }

        public void OnEaseInDone()
        {
            Debug.LogFormat(this, "OnEaseInDone");
        }

        public void ClipActive(float easeWeight)
        {
            Weight = easeWeight;
            Debug.LogFormat(this, "WhileClipIsActive");
        }

        public void OnEaseOutStart(float easeOutDuration)
        {
            Debug.LogFormat(this, "OnEaseOutStarted");
        }

        public void OnClipEnd()
        {
            Debug.LogFormat(this, "OnClipIsDone");
        }
    }
}
