using UnityEngine;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions.Samples
{
    public class InterfaceExampleTwo : MonoBehaviour, ITimeControl
    {
        public void SetTime(double time)
        {
        }


        public void OnControlTimeStart()
        {
            gameObject.SetActive(false);
        }


        public void OnControlTimeStop()
        {
            gameObject.SetActive(true);
        }
    }
}