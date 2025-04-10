using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This is an example of how Timeline can communicate with ScriptableObjects
    ///     A better way would be to implement this in an already existing ScriptableObjectsArchitecture framework, found here:
    ///     https://github.com/solo-fsw/sosxr-unity-scriptableobjectarchitecture
    /// </summary>
    [CreateAssetMenu(fileName = "ControlToUnityEvents", menuName = "SOSXR/TimelineExtensions/ControlUnityEventsSO")]
    public class InterfaceToUnityEventsSO : ScriptableObject, IInterface
    {
        [SerializeField] private UnityEvent m_onClipStart;
        [SerializeField] private UnityEvent m_onEaseInDone;
        [SerializeField] private UnityEvent m_whileClipActive;
        [SerializeField] private UnityEvent m_onEaseOutStarted;
        [SerializeField] private UnityEvent m_onClipEnd;


        public void OnClipStart()
        {
            m_onClipStart?.Invoke();
        }


        public void OnEaseInDone()
        {
            m_onEaseInDone?.Invoke();
        }


        public void ClipActive()
        {
            m_whileClipActive?.Invoke();
        }


        public void OnEaseOutStart()
        {
            m_onEaseOutStarted?.Invoke();
        }


        public void OnClipEnd()
        {
            m_onClipEnd?.Invoke();
        }
    }
}