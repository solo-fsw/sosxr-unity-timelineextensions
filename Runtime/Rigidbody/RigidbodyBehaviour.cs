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
        /// <summary>Value applied to <c>Rigidbody.isKinematic</c> on clip start.</summary>
        public bool IsKinematic;

        /// <summary>Value applied to <c>Rigidbody.useGravity</c> on clip start.</summary>
        public bool UseGravity;

        /// <summary>When true, fires a one-shot force impulse toward <see cref="Target"/> on clip start.</summary>
        public bool AddForce;

        /// <summary>Force magnitude (in the chosen <see cref="ForceMode"/>) applied toward <see cref="Target"/>.</summary>
        public float Amount;

        /// <summary>Transform to aim the force impulse toward. Only used when <see cref="AddForce"/> is true.</summary>
        public Transform Target;

        public ForceMode ForceMode;
    }
}
