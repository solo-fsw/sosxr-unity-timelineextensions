using System;
using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data for the Rigidbody track. Controls kinematic state, gravity, and optionally applies force toward a
    ///     target Transform. <see cref="ForceMode.Impulse" /> fires once on clip start; all other modes apply continuously,
    ///     scaled by the ease weight.
    /// </summary>
    [Serializable]
    public class RigidbodyBehaviour : Behaviour
    {
        /// <summary>Value applied to <c>Rigidbody.isKinematic</c> on clip start.</summary>
        public bool IsKinematic;

        /// <summary>Value applied to <c>Rigidbody.useGravity</c> on clip start.</summary>
        public bool UseGravity;

        /// <summary>
        ///     When true, applies force toward <see cref="Target" />.
        ///     <see cref="ForceMode.Impulse" /> fires once on clip start; all other modes apply continuously each frame, scaled
        ///     by the ease weight.
        /// </summary>
        public bool AddForce;

        /// <summary>Force magnitude (in the chosen <see cref="ForceMode"/>) applied toward <see cref="Target"/>.</summary>
        public float Amount;

        /// <summary>Transform to aim the force impulse toward. Only used when <see cref="AddForce"/> is true.</summary>
        public Transform Target;

        public ForceMode ForceMode;
    }
}
