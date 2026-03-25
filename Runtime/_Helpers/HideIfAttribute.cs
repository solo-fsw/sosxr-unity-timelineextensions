using UnityEngine;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Hides a serialized field in the Inspector when a named bool property on the same component equals
    ///     <paramref name="compareValue"/> (default <c>true</c>).
    /// </summary>
    public class HideIfAttribute : PropertyAttribute
    {
        /// <param name="conditionProperty">Name of the bool property/field on the same MonoBehaviour to evaluate.</param>
        /// <param name="compareValue">The value the property must equal for the field to be hidden. Defaults to true.</param>
        public HideIfAttribute(string conditionProperty, object compareValue = null)
        {
            ConditionProperty = conditionProperty;
            CompareValue = compareValue;
        }

        public string ConditionProperty { get; }
        public object CompareValue { get; }
    }
}
