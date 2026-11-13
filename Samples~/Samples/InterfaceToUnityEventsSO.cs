using UnityEngine;
using UnityEngine.Events;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This is an example of how Timeline can communicate with ScriptableObjects
    ///     A better way would be to implement this in an already existing ScriptableObjectsArchitecture framework, found here:  https://github.com/solo-fsw/sosxr-unity-scriptableobjectarchitecture
    /// </summary>
    [CreateAssetMenu(
        fileName = "InterfaceToUnityEvents",
        menuName = "SOSXR/TimelineExtensions/InterfaceToUnityEventsSO"
    )]
    public class InterfaceToUnityEventsSO : ScriptableObject, IInterface
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

        public void OnClipStart(float ease)
        {
            m_onClipStart?.Invoke(ease);
        }

        public void OnEaseInDone()
        {
            m_onEaseInDone?.Invoke();
        }

        public void ClipActive(float easeWeight)
        {
            m_whileClipActive?.Invoke(easeWeight);
        }

        public void OnEaseOutStart(float ease)
        {
            m_onEaseOutStarted?.Invoke(ease);
        }

        public void OnClipEnd()
        {
            m_onClipEnd?.Invoke();
        }
    }
}
