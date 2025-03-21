using System;
using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    /// <summary>
    ///     Derive from this to use easier custom methods for creating better Editor Windows
    /// </summary>
    public abstract class EditorGUIHelpers : UnityEditor.Editor
    {
        protected GUIStyle DefaultBoxStyle;
        protected GUIStyle AlternateBoxStyle;
        protected GUIStyle LabelStyle;
        protected GUIStyle SmallFontButtonStyle;
        protected GUIStyle TitleStyle;

        protected bool EnableDefaultInspector = false;

        protected UnityEditor.Editor InternalEditor;

        protected const int DefaultSmallSpace = 5;
        protected const int DefaultLargeSpace = 20;
        private const int DefaultCheckBoxWidth = 15;

        protected const float ButtonWidth = 150f;


        protected void GetInternalEditor(string typeName)
        {
            if (InternalEditor != null)
            {
                return;
            }

            var fullName = $"UnityEditor.{typeName}, UnityEditor";
            var editorType = Type.GetType(fullName);

            if (editorType == null)
            {
                Debug.LogWarning($"Could not find the {typeName} type using reflection.");

                return;
            }

            InternalEditor = CreateEditor(target, editorType);
        }


        protected void GetInternalEditor(Type editorType)
        {
            if (InternalEditor != null)
            {
                return;
            }

            if (editorType == null)
            {
                Debug.LogWarning("Editor type is null.");

                return;
            }

            InternalEditor = CreateEditor(target, editorType);
        }


        /// <summary>
        ///     This is sealed to force derived classes to use OnInspectorGUIHelpers
        /// </summary>
        public override void OnInspectorGUI()
        {
            InitDefaultBoxStyle();
            InitAlternateBoxStyle();
            InitLabelStyle();
            InitLargeWhiteStyle();
            InitSmallFontButtonStyle();

            serializedObject.Update(); // Get latest values form serializedObject

            if (EnableDefaultInspector)
            {
                DrawDefaultInspector();
                CreateCustomInspectorToggleButtons();
            }
            else
            {
                CreateScriptField(target);

                if (InternalEditor == null)
                {
                    return;
                }

                InternalEditor.OnInspectorGUI();

                AddSOSXRHeader();

                CustomInspectorContent();
            }

            serializedObject.ApplyModifiedProperties(); // Needed to allow a List to be populated
        }


        protected static void AddSOSXRHeader()
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIStyle HeaderStyle = new()
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = {textColor = Color.white}
            };

            GUILayout.Label("SOSXR Tools", HeaderStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
        }


        protected abstract void CustomInspectorContent();


        protected void CreateCustomInspectorToggleButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (!EnableDefaultInspector && CreateButton("Default Inspector", SmallFontButtonStyle))
            {
                EnableDefaultInspector = true;
            }

            if (EnableDefaultInspector && CreateButton("Custom Inspector", SmallFontButtonStyle))
            {
                EnableDefaultInspector = false;
            }

            GUILayout.EndHorizontal();
        }


        private void InitLabelStyle()
        {
            LabelStyle = new GUIStyle(EditorStyles.boldLabel);
        }


        private void InitDefaultBoxStyle()
        {
            DefaultBoxStyle = new GUIStyle("box");
        }


        /// <summary>
        ///     Currently this is not yet alternate, I want to sort this out later
        /// </summary>
        private void InitAlternateBoxStyle()
        {
            AlternateBoxStyle = new GUIStyle("box");
        }


        private void InitLargeWhiteStyle()
        {
            TitleStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = Color.white
                },
                fontSize = 16
            };
        }


        private void InitSmallFontButtonStyle()
        {
            SmallFontButtonStyle = new GUIStyle(GUI.skin.button) // Make sure to set it as 'NEW guistyle', otherwise settings will influence the 'base' GUIStyle
            {
                fontSize = 10
            };
        }


        /// <summary>
        ///     Adapted from:
        ///     https://stackoverflow.com/questions/52773961/show-the-script-field-in-a-custom-unity-inspector-for-statemachinebehaviours
        /// </summary>
        /// <param name="editorScriptTarget"></param>
        /// <typeparam name="T"></typeparam>
        private static void CreateScriptField<T>(T editorScriptTarget)
        {
            EditorGUI.BeginDisabledGroup(true);

            if (editorScriptTarget as MonoBehaviour == true)
            {
                EditorGUILayout.ObjectField("Custom Editor for Script:", MonoScript.FromMonoBehaviour(editorScriptTarget as MonoBehaviour), editorScriptTarget.GetType(), false);
            }
            else if (editorScriptTarget as ScriptableObject == true)
            {
                EditorGUILayout.ObjectField("Custom Editor for Script:", MonoScript.FromScriptableObject(editorScriptTarget as ScriptableObject), editorScriptTarget.GetType(), false);
            }

            EditorGUI.EndDisabledGroup();
        }


        /// <summary>
        ///     Set the 'Action method' as the method that needs to have the default layout applied to is
        /// </summary>
        /// <param name="methodName"></param>
        protected void DefaultVerticalBoxedLayout(Action methodName)
        {
            GUILayout.Space(DefaultSmallSpace);
            GUILayout.BeginVertical(DefaultBoxStyle);

            methodName();

            GUILayout.EndVertical();
        }


        /// <summary>
        ///     Set the 'Action method' as the method that needs to have the default layout applied to is
        /// </summary>
        /// <param name="methodName"></param>
        protected void AlternateVerticalBoxedLayout(Action methodName)
        {
            GUILayout.Space(DefaultSmallSpace);
            GUILayout.BeginVertical(DefaultBoxStyle);

            methodName();

            GUILayout.EndVertical();
        }


        protected static void CreateSpace(int size)
        {
            EditorGUILayout.Space(size);
        }


        protected static bool CreateButton(string text, GUIStyle style)
        {
            return GUILayout.Button(text, style);
        }


        protected static bool CreateButton(string text, int buttonWidth)
        {
            return GUILayout.Button(text, GUI.skin.button, GUILayout.Width(DefaultCheckBoxWidth * buttonWidth));
        }


        protected static void CreateHeader(string titleName, GUIStyle style)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(titleName, style);

            GUILayout.EndHorizontal();
        }


        protected bool CreateHeaderToggle(string propertyName, string titleName)
        {
            GUILayout.BeginHorizontal();

            var serializedProperty = serializedObject.FindProperty(propertyName);
            serializedProperty.boolValue = EditorGUILayout.ToggleLeft("", serializedProperty.boolValue, GUILayout.Width(DefaultCheckBoxWidth));

            EditorGUILayout.LabelField(titleName, LabelStyle);

            GUILayout.EndHorizontal();

            return serializedProperty.boolValue;
        }


        protected void CreateFloatField(string propertyName, string toolTip = "")
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.floatValue = EditorGUILayout.FloatField(new GUIContent(serializedProperty.name, toolTip), serializedProperty.floatValue);
        }


        protected void CreateNestedEditor(string propertyName, string toolTip = "")
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            CreateEditor(serializedProperty.objectReferenceValue).OnInspectorGUI();
        }


        protected void CreateFloatSliderProperty(string propertyName, string toolTip = "", float minValue = 0f, float maxValue = 10f)
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.floatValue = EditorGUILayout.Slider(new GUIContent(serializedProperty.name, toolTip), serializedProperty.floatValue, minValue, maxValue);
        }


        protected void CreateIntField(string propertyName, string toolTip = "")
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.intValue = EditorGUILayout.IntField(new GUIContent(serializedProperty.name, toolTip), serializedProperty.intValue);
        }


        protected void CreateIntSliderProperty(string propertyName, string toolTip = "", int minValue = 0, int maxValue = 10)
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.intValue = EditorGUILayout.IntSlider(new GUIContent(serializedProperty.name, toolTip), serializedProperty.intValue, minValue, maxValue);
        }


        protected void CreateAnimationCurveField(string propertyName, string toolTip = "", int start = 0, float startValue = 0f, int end = 1, float endValue = 1f)
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            var defaultAnimationCurve = new AnimationCurve(new Keyframe(start, startValue), new Keyframe(end, endValue));

            serializedProperty.animationCurveValue = EditorGUILayout.CurveField(new GUIContent(serializedProperty.name, toolTip), serializedProperty.animationCurveValue ?? defaultAnimationCurve);
        }


        protected void CreateVector3Field(string propertyName, string toolTip = "")
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.vector3Value = EditorGUILayout.Vector3Field(new GUIContent(serializedProperty.name, toolTip), serializedProperty.vector3Value);
        }


        protected void CreatePropertyField(string propertyName, string toolTip = "", bool includeChildren = true)
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            EditorGUILayout.PropertyField(serializedProperty, new GUIContent(serializedProperty.name, toolTip), includeChildren);
        }


        protected SerializedProperty CreateToggleProperty(string propertyName, string toolTip = "")
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.boolValue = EditorGUILayout.Toggle(new GUIContent(serializedProperty.name, toolTip), serializedProperty.boolValue);

            return serializedProperty;
        }


        protected void CreateColorField(string propertyName, bool hdr = true, string toolTip = "")
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.colorValue = EditorGUILayout.ColorField(new GUIContent(serializedProperty.name, toolTip), serializedProperty.colorValue, true, true, hdr);
        }


        protected void CreateObjectField(string propertyName, Type type, string toolTip = "", bool allowSceneObjects = true)
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(serializedProperty.name, toolTip), serializedProperty.objectReferenceValue, type, allowSceneObjects);
        }


        protected void CreateStringField(string propertyName, string toolTip = "")
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.stringValue = EditorGUILayout.TextField(new GUIContent(serializedProperty.name, toolTip), serializedProperty.stringValue);
        }


        protected void CreateEnumField(string propertyName, Type enumType, string toolTip = "")
        {
            var serializedProperty = serializedObject.FindProperty(propertyName);

            serializedProperty.enumValueIndex = EditorGUILayout.Popup(new GUIContent(serializedProperty.name, toolTip), serializedProperty.enumValueIndex, Enum.GetNames(enumType));
        }


        protected void OnDisable()
        {
            if (InternalEditor == null)
            {
                return;
            }

            DestroyImmediate(InternalEditor);
        }
    }
}