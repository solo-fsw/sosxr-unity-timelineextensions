using System;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data for the Animator track. Holds the state names to cross-fade to at clip start and ease-out.
    /// </summary>
    [Serializable]
    public class AnimatorBehaviour : Behaviour
    {
        /// <summary>The Animator state to cross-fade to when this clip starts. Leave empty to skip.</summary>
        public string StartClipStateName = "";

        /// <summary>The Animator state to cross-fade to when this clip's ease-out begins. Defaults to the Animator Controller's entry state.</summary>
        public string EndClipStateName = "Default_State";
    }
}
