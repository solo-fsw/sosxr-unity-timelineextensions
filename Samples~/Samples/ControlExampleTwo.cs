using UnityEngine;


namespace SOSXR.TimelineExtensions.Samples
{
    public class ControlExampleTwo : MonoBehaviour, IControl
    {
        public void OnClipStart()
        {
            gameObject.SetActive(false);
        }


        public void OnEaseInDone()
        {
        }


        public void ClipActive()
        {
        }


        public void OnEaseOutStart()
        {
        }


        public void OnClipEnd()
        {
            gameObject.SetActive(true);
        }
    }
}