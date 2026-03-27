using System;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Mark a Track with this attribute to indicate it does not require a binding.
    ///     The default "There is nothing bound to this Track" warning will be suppressed for tracks with this attribute.
    ///     Use this for tracks where each clip carries its own references (e.g., PostProcessingTrack, InterfaceTrack).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NoTrackBindingAttribute : Attribute { }
}
