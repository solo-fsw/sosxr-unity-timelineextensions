using System;
using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class ToTargetBehaviour : Behaviour
    {
        [Tooltip("Which axes to use for movement calculations. 0 = ignore, 1 = use.")]
        public Vector3Int AxisToUse = new(1, 0, 1);

        [Tooltip(
            "Movement speed in units per second. Used to calculate clip duration in the editor."
        )]
        public float MoveSpeed = 2f;

        [Tooltip("Rotation slerp speed toward the direction of travel.")]
        public float RotateSpeed = 3f;

        public Vector3 StartPosition { get; set; }
        public Vector3 TargetPosition { get; set; }
        public bool StartPositionCaptured { get; set; }

        /// <summary>Pre-computed integral of easeWeight over full clip duration. Equals distance/MoveSpeed. Set by the clip.</summary>
        public float TotalEaseWeightIntegral { get; set; }

        /// <summary>Accumulated easeWeight * deltaTime each frame. Divided by TotalEaseWeightIntegral to get NormalizedPosition.</summary>
        public float AccumulatedEaseTime { get; set; }

        /// <summary>Ease-weighted 0-1 position along the path. 0 = start, 1 = target. Set by the mixer each frame.</summary>
        public float NormalizedPosition { get; set; }
    }
}
