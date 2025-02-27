using UnityEngine;
using UnityEngine.Events;


public class ITLTransformUnityEvent : MonoBehaviour, ITLActivate
{
    [SerializeField] private UnityEvent<Transform> m_eventToFire;
    [SerializeField] private Transform m_transformToPass;


    public bool IsValid { get; private set; }


    public void TLActivate()
    {
        FireEvent();
    }


    public void OnValidate()
    {
        IsValid = true;
    }


    public void FireEvent()
    {
        m_eventToFire?.Invoke(m_transformToPass);
    }
}