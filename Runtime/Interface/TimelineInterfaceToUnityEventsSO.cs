using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This is an example of how Timeline can communicate with ScriptableObjects
    ///     A better way would be to implement this in the ScriptableObjecsArchitecture framework, found here:
    ///     https://github.com/solo-fsw/sosxr-unity-scriptableobjectarchitecture
    ///     documentation: https://docs.unity3d.com/Packages/com.unity.timeline@1.8/api/UnityEngine.Timeline.ITimeControl.html
    /// </summary>
    [CreateAssetMenu(fileName = "TimelineInterfaceToUnityEvents", menuName = "SOSXR/TimelineExtensions/TimelineInterfaceToUnityEventsSO")]
    public class TimelineInterfaceToUnityEventsSO : ScriptableObject, ITimeControl
    {
        [SerializeField] private UnityEvent m_onClipStart;
        [SerializeField] private UnityEvent<double> m_onClipUpdate;
        [SerializeField] private UnityEvent m_onClipEnd;


        /// <summary>
        ///     Called when the associated Timeline clip becomes active.
        /// </summary>
        public void OnControlTimeStart()
        {
            m_onClipStart?.Invoke();
        }


        /// <summary>
        ///     Called each frame the Timeline clip is active.
        /// </summary>
        /// <param name="time">The local time of the associated Timeline clip.</param>
        public void SetTime(double time)
        {
            m_onClipUpdate?.Invoke(time);
        }


        /// <summary>
        ///     Called when the associated Timeline clip becomes deactivated.
        /// </summary>
        public void OnControlTimeStop()
        {
            m_onClipEnd?.Invoke();
        }
    }
}