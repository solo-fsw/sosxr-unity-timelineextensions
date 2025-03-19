using System;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     This acts as our data for the clip to write to
    ///     Adapted from GameDevGuide: https://youtu.be/12bfRIvqLW4
    /// </summary>
    [Serializable]
    public class AnimatorBehaviour : Behaviour
    {
        public string StartClipStateName = "";

        public string EndClipStateName = "Default_State";
    }
}