/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext
{
    public static partial class Prefs
    {
        public static KeyCode duplicateToolKeyCode = KeyCode.U;
        public static EventModifiers duplicateToolModifiers = EventModifiers.None;

        private class DuplicateToolManager : StandalonePrefManager<DuplicateToolManager>, IHasShortcutPref
        {
            public override IEnumerable<string> keywords
            {
                get
                {
                    return new[]
                    {
                        "Duplicate",
                        "Tool"
                    };
                }
            }

            public override void Draw()
            {
                GUILayout.Label("Duplicate Tool");
                EditorGUI.indentLevel++;

                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = labelWidth + 5;
                duplicateToolKeyCode = (KeyCode)EditorGUILayout.EnumPopup("Hot Key", duplicateToolKeyCode, GUILayout.Width(420));
                EditorGUIUtility.labelWidth = oldLabelWidth;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(16);
                GUILayout.Label("Modifiers", GUILayout.Width(modifierLabelWidth + 15));
                duplicateToolModifiers = DrawModifiers(duplicateToolModifiers, true);
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }

            public IEnumerable<Shortcut> GetShortcuts()
            {
                List<Shortcut> shortcuts = new List<Shortcut>
                {
                    new Shortcut("Select Duplicate Tool", "Scene View", duplicateToolModifiers, duplicateToolKeyCode)
                };
                return shortcuts;
            }
        }
    }
}