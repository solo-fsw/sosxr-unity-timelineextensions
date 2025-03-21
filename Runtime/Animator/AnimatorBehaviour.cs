using System;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This acts as our data for the clip to write to
    /// </summary>
    [Serializable]
    public class AnimatorBehaviour : Behaviour
    {
        public string StartClipStateName = "";
        public string EndClipStateName = "Default_State";
    }
}