using UnityEngine;

namespace SOSXR.TimelineExtensions.Samples
{
    public class InterfaceExampleTwo : MonoBehaviour, IInterface
    {
        public void OnClipStart()
        {
            gameObject.SetActive(false);
        }

        public void OnEaseInDone() { }

        public void ClipActive(float easeWeight) { }

        public void OnEaseOutStart() { }

        public void OnClipEnd()
        {
            gameObject.SetActive(true);
        }
    }
}

