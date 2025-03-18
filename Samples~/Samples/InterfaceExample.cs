using UnityEngine;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions.Samples
{
    public class InterfaceExample : MonoBehaviour, ITimeControl
    {
        /// <summary>
        ///     Called when the associated Timeline clip becomes active.
        /// </summary>
        public void OnControlTimeStart()
        {
            Debug.Log("Clip is starting");
        }


        /// <summary>
        ///     Called each frame the Timeline clip is active.
        /// </summary>
        /// <param name="time">The local time of the associated Timeline clip.</param>
        public void SetTime(double time)
        {
            Debug.Log("Clip is active");
        }


        /// <summary>
        ///     Called when the associated Timeline clip becomes deactivated.
        /// </summary>
        public void OnControlTimeStop()
        {
            Debug.Log("Clip is ending");
        }
    }
}