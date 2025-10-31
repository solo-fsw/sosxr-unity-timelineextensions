using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public class HideIfAttribute : PropertyAttribute
    {
        public HideIfAttribute(string conditionProperty, object compareValue = null)
        {
            ConditionProperty = conditionProperty;
            CompareValue = compareValue;
        }


        public string ConditionProperty { get; }
        public object CompareValue { get; }
    }
}