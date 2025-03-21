using System;


namespace SOSXR.TimelineExtensions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        public ButtonAttribute(string label = null)
        {
            Label = label;
        }


        public string Label { get; }
    }
}