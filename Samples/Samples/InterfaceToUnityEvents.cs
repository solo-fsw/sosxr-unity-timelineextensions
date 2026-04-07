using UnityEngine;
using UnityEngine.Events;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This class is an example of how to use the IControl interface with UnityEvents.
    /// </summary>
    public class InterfaceToUnityEvents : MonoBehaviour, IInterface
    {
        [SerializeField]
        private UnityEvent<float> m_onClipStart;

        [SerializeField]
        private UnityEvent m_onEaseInDone;

        [SerializeField]
        private UnityEvent<float> m_whileClipActive;

        [SerializeField]
        private UnityEvent<float> m_onEaseOutStarted;

        [SerializeField]
        private UnityEvent m_onClipEnd;

        public void OnClipStart(float easeInDuration)
        {
            m_onClipStart?.Invoke(easeInDuration);
        }

        public void OnEaseInDone()
        {
            m_onEaseInDone?.Invoke();
        }

        public void ClipActive(float easeWeight)
        {
            m_whileClipActive?.Invoke(easeWeight);
        }

        public void OnEaseOutStart(float easeOutDuration)
        {
            m_onEaseOutStarted?.Invoke(easeOutDuration);
        }

        public void OnClipEnd()
        {
            m_onClipEnd?.Invoke();
        }
    }
}

