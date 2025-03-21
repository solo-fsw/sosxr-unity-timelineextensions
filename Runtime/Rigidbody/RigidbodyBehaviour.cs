using System;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class RigidbodyBehaviour : Behaviour
    {
        public bool isKinematic;
        public bool useGravity;
        public bool addForce;
        public float amount;
        public Transform target;
        public ForceMode forceMode;
    }
}