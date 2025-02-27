using System.Collections.Generic;
using UnityEngine;


public class MassEnabler : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_gameObjectsToEnable;
    public bool IsValid { get; private set; }


    public void OnValidate()
    {
        IsValid = true;
    }


    public void EnableAll()
    {
        foreach (var go in m_gameObjectsToEnable)
        {
            go.SetActive(true);
        }
    }
}