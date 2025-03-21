using System;
using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class RotateToTargetBehaviour : Behaviour
    {
        public Vector3Int AxisToUse = new(1, 0, 1);

        public Transform Rotator { get; set; }
        public float EaseSpeed { get; set; }
    }
}