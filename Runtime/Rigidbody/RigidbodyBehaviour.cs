using System;
using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data for the Rigidbody track. Controls kinematic state, gravity, and optionally applies a one-shot force
    ///     toward a target Transform when the clip starts.
    /// </summary>
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
