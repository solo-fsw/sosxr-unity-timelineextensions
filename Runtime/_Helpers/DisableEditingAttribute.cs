using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Marks a serialized field as read-only in the Inspector — it is visible but cannot be edited.
    ///     Useful for exposing computed or runtime values without allowing direct modification.
    ///     From: <a href="https://gist.github.com/LotteMakesStuff/c0a3b404524be57574ffa5f8270268ea">LotteMakesStuff</a>.
    /// </summary>
    public class DisableEditingAttribute : PropertyAttribute
    {
    }
}
