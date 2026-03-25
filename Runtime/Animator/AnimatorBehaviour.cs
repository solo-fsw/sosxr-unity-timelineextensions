using System;
using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    [Serializable]
    public class AnimatorBehaviour : Behaviour
    {
        [Tooltip("The Animator state to blend to when this clip is active")]
        public string StateName = "";
    }
}
