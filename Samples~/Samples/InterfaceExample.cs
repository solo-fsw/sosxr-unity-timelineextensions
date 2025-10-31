using UnityEngine;


namespace SOSXR.TimelineExtensions.Samples
{
    public class InterfaceExample : MonoBehaviour, IInterface
    {
        public void OnClipStart()
        {
            Debug.LogFormat(this, "OnClipStart");
        }


        public void OnEaseInDone()
        {
            Debug.LogFormat(this, "OnEaseInDone");
        }


        public void ClipActive()
        {
            Debug.LogFormat(this, "WhileClipIsActive");
        }


        public void OnEaseOutStart()
        {
            Debug.LogFormat(this, "OnEaseOutStarted");
        }


        public void OnClipEnd()
        {
            Debug.LogFormat(this, "OnClipIsDone");
        }
    }
}