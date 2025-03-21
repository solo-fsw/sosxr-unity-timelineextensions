using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Act as our data for the clip to write to
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [Serializable]
    public class RigBehaviour : Behaviour
    {
        public GameObject ConstraintObject;
        public IRigConstraint Constraint;
        public float RigWeight { get; set; }
        public float ConstraintWeight { get; set; }
        public WeightType WeightType { get; set; }
        public bool MatchWeightOnClipStart { get; set; }
    }
}