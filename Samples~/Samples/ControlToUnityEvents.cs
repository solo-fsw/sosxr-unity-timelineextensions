using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This class is an example of how to use the IControl interface with UnityEvents.
    /// </summary>
    public class ControlToUnityEvents : MonoBehaviour, IControl
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