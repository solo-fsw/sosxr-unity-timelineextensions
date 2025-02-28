using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SOSXR.TimelineExtensions.Editor
{
    public static class InspectorCommon
    {
        public enum AlertType
        {
            Good,
            Normal,
            Warning,
            Error
        }


        public enum HeaderType
        {
            Level1,
            Level2,
            Level3
        }


        public static Color ClrHeaderWarn => ConvertColor(193, 164, 36);

        public static Color ClrBgError => ConvertColor(249, 132, 132);

        public static Color ClrBgWarn => ConvertColor(251, byte.MaxValue, 167);

        public static Color ClrBgNormal => ConvertColor(167, 241, byte.MaxValue);

        public static Color ClrBgGood => ConvertColor(51, 229, 167);

        public static Color ClrBgButton => ConvertColor(59, 91, 133);

        public static Color ClrHeaderLvl1 => ConvertColor(95, 148, 217);

        public static Color ClrHeaderLvl2 => ConvertColor(57, 93, 142);

        public static Color ClrHeaderLvl3 => ConvertColor(46, 62, 77);

        public static Color ClrHeaderLvl3Disabled => ConvertColor(150, 150, 150);
        public static Color InspectorTextColor { get; } = new GUIStyle().normal.textColor;

        public static GUIStyle StyleHasBackground
        {
            get
            {
                if (styleHasBackground.normal.background == null)
                {
                    styleHasBackground.normal.background = new Texture2D(1, 1);
                }

                if (styleHasBackground.padding.left != 5)
                {
                    styleHasBackground.padding = new RectOffset(5, 5, 3, 3);
                }

                return styleHasBackground;
            }
        }

        public static GUIStyle StyleLeftAlign
        {
            get
            {
                styleLeftAlign.alignment = TextAnchor.MiddleLeft;

                return styleLeftAlign;
            }
        }

        public static GUIStyle StyleBoldCentered
        {
            get
            {
                styleBoldCentered.normal.background = new Texture2D(1, 1);
                styleBoldCentered.alignment = TextAnchor.MiddleCenter;
                styleBoldCentered.fontStyle = FontStyle.Bold;
                styleBoldCentered.fontSize = 10;

                return styleBoldCentered;
            }
        }

        public static GUIStyle StyleBoldRightLabel
        {
            get
            {
                styleBoldRightLabel.alignment = TextAnchor.LowerRight;
                styleBoldRightLabel.fontStyle = FontStyle.Bold;
                styleBoldRightLabel.padding = new RectOffset(0, 2, 7, 2);
                styleBoldRightLabel.fontSize = 11;

                return styleBoldRightLabel;
            }
        }

        public static GUIStyle StyleBoxZeroPadLeft
        {
            get
            {
                styleBoxZeroPadLeft.margin = new RectOffset(0, 4, 4, 4);
                styleBoxZeroPadLeft.padding = new RectOffset(0, 3, 3, 3);

                return styleBoxZeroPadLeft;
            }
        }

        public static GUIStyle StyleBoxZeroPadRight
        {
            get
            {
                styleBoxZeroPadRight.margin = new RectOffset(4, 0, 4, 4);
                styleBoxZeroPadRight.padding = new RectOffset(3, 0, 3, 3);

                return styleBoxZeroPadRight;
            }
        }

        public static GUIStyle StyleRightAlign
        {
            get
            {
                styleRightAlign.alignment = TextAnchor.LowerRight;

                return styleRightAlign;
            }
        }


        private static readonly GUIStyle styleHasBackground = new();
        private static readonly GUIStyle styleLeftAlign = new();
        private static readonly GUIStyle styleRightAlign = new();
        private static readonly GUIStyle styleBoldCentered = new();
        private static readonly GUIStyle styleBoldRightLabel = new();
        private static readonly GUIStyle styleBoxZeroPadLeft = new("Box");
        private static readonly GUIStyle styleBoxZeroPadRight = new("Box");


        public static bool DrawSectionHeader(
            string title,
            HeaderType type,
            bool state,
            bool configGood,
            string xtraMsg = "",
            bool enabled = true)
        {
            var backgroundColor = GUI.backgroundColor;
            title = (state ? "▼ " : "► ") + title;
            xtraMsg = "<color=#ffffffff>" + xtraMsg + "</color>";
            string str;
            Color color;

            switch (type)
            {
                case HeaderType.Level1:
                    str = "12";
                    color = ClrHeaderLvl1;
                    title = "<b>" + title + "</b>";

                    break;
                case HeaderType.Level2:
                    str = "11";
                    color = ClrHeaderLvl2;
                    title = (configGood ? "<color=#ddddddff>" : "<color=#000000ff>") + title + "</color>";

                    break;
                default:
                    str = "10";
                    color = enabled ? ClrHeaderLvl3 : ClrHeaderLvl3Disabled;
                    title = (configGood ? "<color=#ddddddff>" : "<color=#000000ff>") + title + "</color>";

                    break;
            }

            xtraMsg = "<size=" + str + ">" + xtraMsg + "</size>";
            title = "<size=" + str + ">" + title + "</size>";

            if (!configGood)
            {
                GUI.backgroundColor = state ? ClrHeaderWarn * 1.2f : ClrHeaderWarn;
                title += " (config required)";
            }
            else
            {
                GUI.backgroundColor = state ? color * 1.2f : color;
            }

            GUILayout.BeginHorizontal(StyleHasBackground);

            if (!GUILayout.Toggle(true, title, StyleLeftAlign, GUILayout.MinWidth(40f), GUILayout.ExpandWidth(true)))
            {
                state = !state;
            }

            if (configGood && !GUILayout.Toggle(true, xtraMsg, StyleRightAlign))
            {
                state = !state;
            }

            GUILayout.EndHorizontal();
            GUI.backgroundColor = backgroundColor;

            return state;
        }


        public static void DrawStatusMessageBlock(string msg, string tip, Color clr)
        {
            GUILayout.Space(5f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            var style = new GUIStyle();
            var textColor = style.normal.textColor;
            style.wordWrap = true;
            style.normal.textColor = clr;
            GUILayout.Label(new GUIContent(msg, tip), style);
            style.normal.textColor = textColor;
            GUILayout.EndVertical();
            GUILayout.Space(5f);
        }


        public static void DrawBackgroundCondition(bool isGood)
        {
            if (isGood)
            {
                GUI.backgroundColor = ClrBgGood;
            }
            else
            {
                GUI.backgroundColor = ClrBgError;
            }
        }


        public static bool DrawLabelToggle(string label, bool state, GUIStyle option)
        {
            if (!GUILayout.Toggle(true, new GUIContent(label, "Click to toggle feature."), option))
            {
                state = !state;
            }

            return state;
        }


        public static void DrawItemNonButton(string text, string tip, float width = 25f)
        {
            GUILayout.Label(new GUIContent(text, tip), EditorStyles.toolbarButton, GUILayout.Width(width));
        }


        public static bool DrawFunctionButton(GUIContent content, int size, Color bg, float width)
        {
            GUI.backgroundColor = bg;

            return GUILayout.Button(content, new GUIStyle(GUI.skin.button)
            {
                fontSize = size,
                normal =
                {
                    textColor = Color.white
                }
            }, GUILayout.MaxWidth(width));
        }


        public static bool DrawItemButton(string text, string tip, float width = 25f, float height = 18f)
        {
            return GUILayout.Button(new GUIContent(text, tip), EditorStyles.toolbarButton, GUILayout.Width(width), GUILayout.Height(height));
        }


        public static void DrawBackgroundCondition(AlertType alertType)
        {
            switch (alertType)
            {
                case AlertType.Good:
                    GUI.backgroundColor = ClrBgGood;

                    break;
                case AlertType.Warning:
                    GUI.backgroundColor = ClrBgWarn;

                    break;
                case AlertType.Error:
                    GUI.backgroundColor = ClrBgError;

                    break;
                default:
                    GUI.backgroundColor = ClrBgNormal;

                    break;
            }
        }


        public static Color ConvertColor(int r, int g, int b, int a = 255)
        {
            return new Color(r / (float) byte.MaxValue, g / (float) byte.MaxValue, b / (float) byte.MaxValue, a / (float) byte.MaxValue);
        }


        public static void DrawDoubleSlider(
            string label,
            string shortLabel,
            string tooltip,
            float inspWidth,
            ref float loCutoff,
            ref float hiCutoff)
        {
            GUILayout.BeginHorizontal();
            var num = 330f;
            var maxWidth1 = 50f;
            var maxWidth2 = inspWidth < (double) num ? 87f : 125f;
            loCutoff = EditorGUILayout.FloatField(loCutoff, GUILayout.MaxWidth(maxWidth1));
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(inspWidth < (double) num ? shortLabel : label, tooltip), GUILayout.MaxWidth(maxWidth2));
            GUILayout.FlexibleSpace();
            hiCutoff = EditorGUILayout.FloatField(hiCutoff, GUILayout.MaxWidth(maxWidth1));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref loCutoff, ref hiCutoff, 0.0f, 1f);
        }


        public static void DrawHiLoFloat(float inspWidth, ref float loCutoff, ref float hiCutoff)
        {
            GUILayout.BeginHorizontal();
            var labelWidth = EditorGUIUtility.labelWidth;
            var fieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = 40f;
            EditorGUIUtility.fieldWidth = 40f;
            GUILayout.FlexibleSpace();
            loCutoff = EditorGUILayout.FloatField("From", Mathf.Clamp(loCutoff, 0.0f, hiCutoff - 1f / 1000f));
            GUILayout.Space(15f);
            hiCutoff = EditorGUILayout.FloatField("To", Mathf.Max(hiCutoff, loCutoff + 1f / 1000f));
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.fieldWidth = fieldWidth;
            EditorGUIUtility.labelWidth = labelWidth;
        }


        public static void DrawResetBg()
        {
            GUI.backgroundColor = Color.white;
        }


        private static string[] CheckDragDrop<T>(List<T> list)
        {
            var rect = GUILayoutUtility.GetRect(0.0f, 25f, GUILayout.ExpandWidth(true));
            GUI.backgroundColor = list.Count != 0 ? ClrBgGood : ClrBgError;
            GUI.Box(rect, "Drag and Drop 2D Elements Here", StyleBoldCentered);
            DrawResetBg();
            var current = Event.current;

            if (rect.Contains(current.mousePosition))
            {
                switch (current.type)
                {
                    case EventType.DragUpdated:
                        var flag = false;

                        foreach (var path in DragAndDrop.paths)
                        {
                            if (AssetDatabase.LoadAssetAtPath(path, typeof(T)) == null)
                            {
                                flag = true;

                                break;
                            }
                        }

                        DragAndDrop.visualMode = !flag ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

                        break;
                    case EventType.DragPerform:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (current.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            return DragAndDrop.paths;
                        }

                        break;
                }
            }

            return null;
        }
    }
}