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
        public bool IsKinematic;
        public bool UseGravity;
        public bool AddForce;
        public float Amount;
        public Transform Target;
        public ForceMode ForceMode;
    }
}
