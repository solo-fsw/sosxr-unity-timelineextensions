using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This is an example of how Timeline can communicate with ScriptableObjects
    ///     A better way would be to implement this in an already existing ScriptableObjectsArchitecture framework, found here:
    ///     https://github.com/solo-fsw/sosxr-unity-scriptableobjectarchitecture
    /// </summary>
    [CreateAssetMenu(fileName = "InterfaceToUnityEvents", menuName = "SOSXR/TimelineExtensions/InterfaceToUnityEventsSO")]
    public class InterfaceToUnityEventsSO : ScriptableObject, ITimeControl
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