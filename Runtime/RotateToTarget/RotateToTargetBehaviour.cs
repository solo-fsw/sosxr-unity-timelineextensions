using System;
using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data for the RotateToTarget track. Specifies which axes to consider and the ease/slerp speed.
    ///     <see cref="Rotator"/> and <see cref="EaseSpeed"/> are populated by the clip at playable-creation time.
    /// </summary>
    [Serializable]
    public class RotateToTargetBehaviour : Behaviour
    {
        /// <summary>Axes to include in direction calculations. Set a component to 0 to ignore that axis (e.g. y=0 for horizontal-only rotation).</summary>
        public Vector3Int AxisToUse = new(1, 0, 1);

        public Transform Rotator { get; set; }
    }
}
