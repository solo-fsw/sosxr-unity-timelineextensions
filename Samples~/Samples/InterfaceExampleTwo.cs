using UnityEngine;

namespace SOSXR.TimelineExtensions.Samples
{
    public class InterfaceExampleTwo : MonoBehaviour, IInterface
    {
        public void OnClipStart(float ease)
        {
            gameObject.SetActive(false);
        }

        public void OnEaseInDone() { }

        public void ClipActive(float easeWeight) { }

        public void OnEaseOutStart(float ease) { }

        public void OnClipEnd()
        {
            gameObject.SetActive(true);
        }
    }
}

