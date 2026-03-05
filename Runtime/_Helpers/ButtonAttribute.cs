using System;


namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Method attribute that causes the Editor drawer to render a clickable button in the
    ///     Inspector for the decorated method. Optionally accepts a custom label string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        /// <param name="label">Optional Inspector button label. Defaults to the method name when null.</param>
        public ButtonAttribute(string label = null)
        {
            Label = label;
        }


        public string Label { get; }
    }
}