/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using InfinityCode.uContext.Tools;
using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uContext
{
    public static partial class Prefs
    {
        public static bool hierarchyIcons = true;
        public static HierarchyIconsDisplayRule hierarchyIconsDisplayRule = HierarchyIconsDisplayRule.onHoverWithModifiers;
        public static bool hierarchyErrorIcons = true;
        public static bool hierarchyOverrideMainIcon = true;
        public static bool hierarchySoloVisibility = true;
        public static bool hierarchyEnableGameObject = true;

#if !UNITY_EDITOR_OSX
        public static EventModifiers hierarchyIconsModifiers = EventModifiers.Control;
#else
        public static EventModifiers hierarchyIconsModifiers = EventModifiers.Command;
#endif
        public static int hierarchyIconsMaxItems = 6;

        public static bool hierarchyHeaders = true;
        public static string hierarchyHeaderPrefix = "--";
        public static string hierarchyHeaderTrimChars = " -=";
        public static Color hierarchyHeaderBackground = Color.gray;
        public static Color hierarchyHeaderTextColor = Color.white;

        public class HierarchyManager : StandalonePrefManager<HierarchyManager>, IHasShortcutPref
        {
#if !UCONTEXT_PRO
            private const string sectionLabel = "Show Components In Hierarchy (PRO)";
#else
            private const string sectionLabel = "Show Components In Hierarchy";
#endif

            public override IEnumerable<string> keywords
            {
                get { return new[] { "Hierarchy Icons", "Max Items", "Show error icon if GameObject has an error or exception" }; }
            }

            public override float order
            {
                get { return -46; }
            }

            public override void Draw()
            {
                hierarchyEnableGameObject = EditorGUILayout.ToggleLeft("Enable / Disable GameObject", hierarchyEnableGameObject);

                DrawHeaders();

                EditorGUI.BeginChangeCheck();
                hierarchyOverrideMainIcon = EditorGUILayout.ToggleLeft("Show Best Component Icon Before Name", hierarchyOverrideMainIcon);
                if (EditorGUI.EndChangeCheck())
                {
                    Object[] windows = UnityEngine.Resources.FindObjectsOfTypeAll(SceneHierarchyWindowRef.type);
                    foreach (Object wnd in windows)
                    {
                        EditorWindow window = wnd as EditorWindow;
                        HierarchyHelper.SetDefaultIconsSize(window, hierarchyOverrideMainIcon ? 0 : 18);
                        window.Repaint();
                    }
                }

                hierarchyIcons = EditorGUILayout.ToggleLeft(sectionLabel, hierarchyIcons, EditorStyles.label);

                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Modifiers", GUILayout.Width(labelWidth - 17));
                hierarchyIconsModifiers = DrawModifiers(hierarchyIconsModifiers);
                EditorGUILayout.EndHorizontal();

                hierarchyIconsMaxItems = EditorGUILayout.IntField("Max Items", hierarchyIconsMaxItems);
                if (hierarchyIconsMaxItems < 1) hierarchyIconsMaxItems = 1;

                EditorGUI.indentLevel--;

                hierarchyErrorIcons = EditorGUILayout.ToggleLeft("Show Error Icon When GameObject Has an Error or Exception", hierarchyErrorIcons);
                hierarchySoloVisibility = EditorGUILayout.ToggleLeft("Solo Visibility", hierarchySoloVisibility);
            }

            private static void DrawHeaders()
            {
                hierarchyHeaders = EditorGUILayout.ToggleLeft("Headers", hierarchyHeaders);
                EditorGUI.BeginDisabledGroup(!hierarchyHeaders);
                EditorGUI.indentLevel++;

                hierarchyHeaderPrefix = EditorGUILayout.TextField("Prefix", hierarchyHeaderPrefix);
                EditorGUI.BeginChangeCheck();
                hierarchyHeaderTrimChars = EditorGUILayout.TextField("Trim Chars", hierarchyHeaderTrimChars);
                if (EditorGUI.EndChangeCheck()) HierarchyHeader.ResetTrimChars();

                EditorGUI.BeginChangeCheck();
                hierarchyHeaderBackground = EditorGUILayout.ColorField("Background", hierarchyHeaderBackground);
                hierarchyHeaderTextColor = EditorGUILayout.ColorField("Text", hierarchyHeaderTextColor);
                if (EditorGUI.EndChangeCheck()) HierarchyHeader.ResetStyle();

                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
            }

            public IEnumerable<Shortcut> GetShortcuts()
            {
                if (!hierarchyIcons) return new Shortcut[0];

                return new[]
                {
                    new Shortcut("Show Component Icons", "Hierarchy", hierarchyIconsModifiers)
                };
            }
        }
    }
}