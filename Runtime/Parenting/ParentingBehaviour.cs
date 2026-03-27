using System;
using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data for the Parenting track. Stores the child transform to reparent, whether to zero out local
    ///     position/rotation on attachment, and a snapshot of the original parent so it can be restored when the clip ends.
    /// </summary>
    [Serializable]
    public class ParentingBehaviour : Behaviour
    {
        [HideInInspector]
        public Transform Child;

        [HideInInspector]
        public KeepPosition KeepPosition;

        [HideInInspector]
        public Transform OriginalParent;

        [HideInInspector]
        public Vector3 OriginalPosition;
    }
}
