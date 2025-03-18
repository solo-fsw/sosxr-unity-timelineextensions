using UnityEngine;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions.Samples
{
    public class InterfaceExample : MonoBehaviour, ITimeControl
    {
        public void SetTime(double time)
        {
            
        }


        public void OnControlTimeStart()
        {
            Debug.Log("Clip is starting");
        }

        public void OnControlTimeStop()
        {
            Debug.Log("Clip is done!");
        }
    }
}