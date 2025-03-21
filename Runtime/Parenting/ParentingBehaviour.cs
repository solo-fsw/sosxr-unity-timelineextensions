using System;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class ParentingBehaviour : Behaviour
    {
        [HideInInspector] public Transform Child;
        [HideInInspector] public bool ZeroInOnParent;

        [HideInInspector] public Transform OriginalParent;
    }
}