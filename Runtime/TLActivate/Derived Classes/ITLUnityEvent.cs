using UnityEngine;
using UnityEngine.Events;


public class ITLUnityEvent : MonoBehaviour, ITLActivate
{
    [SerializeField] private UnityEvent m_eventToFire;


    public void TLActivate()
    {
        FireEvent();
    }


    public bool IsValid { get; private set; }


    public void OnValidate()
    {
        IsValid = true;
    }

[ContextMenu(nameof(FireEvent))]
    public void FireEvent()
    {
        m_eventToFire?.Invoke();
    }
}