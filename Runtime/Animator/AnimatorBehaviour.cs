using System;
using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Per-clip data for the Animator track. Holds the target <see cref="StateName"/> to crossfade to when this clip becomes active.
    /// </summary>
    [Serializable]
    public class AnimatorBehaviour : Behaviour
    {
        [Tooltip("The Animator state to blend to when this clip is active")]
        public string StateName = "";
    }
}
