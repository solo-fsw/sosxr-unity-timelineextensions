using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions.EditorScripts
{
    /// <summary>
    ///     Custom Inspector base for <see cref="Clip"/>-derived assets. Scans the target type for methods decorated with
    ///     <see cref="ButtonAttribute"/> and renders a clickable Inspector button for each one.
    /// </summary>
    [CustomEditor(typeof(Clip), true)]
    public class ButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetType = target.GetType();
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            foreach (var method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();

                if (buttonAttribute == null)
                {
                    continue;
                }

                if (GUILayout.Button(buttonAttribute.Label ?? method.Name))
                {
                    method.Invoke(target, null);
                }
            }
        }
    }
}