/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext
{
    public static partial class Prefs
    {
        public static bool rotateByShortcut = true;

        private class RotateByShortcutManager : StandalonePrefManager<RotateByShortcutManager>, IHasShortcutPref
        {
            public override IEnumerable<string> keywords
            {
                get { return new[] { "Rotate By Shortcut" }; }
            }

            public override void Draw()
            {
#if UCONTEXT_PRO
                string label = "Rotate By Shortcut";
#else
                string label = "Rotate By Shortcut (PRO)";
#endif
                rotateByShortcut = EditorGUILayout.ToggleLeft(label, rotateByShortcut);
            }

            public IEnumerable<Shortcut> GetShortcuts()
            {
#if !UCONTEXT_PRO
                return new Shortcut[0];
#else

                if (!rotateByShortcut) return new Shortcut[0];

#if !UNITY_EDITOR_OSX
                EventModifiers modifiers = EventModifiers.Control | EventModifiers.Shift;
#else
                EventModifiers modifiers = EventModifiers.Command | EventModifiers.Shift;
#endif

                return new[]
                {
                    new Shortcut("Rotate Selection Y -90°", "Scene View", modifiers, KeyCode.LeftArrow),
                    new Shortcut("Rotate Selection Y +90°", "Scene View", modifiers, KeyCode.RightArrow),
                    new Shortcut("Rotate Selection From Myself -90°", "Scene View", modifiers, KeyCode.UpArrow),
                    new Shortcut("Rotate Selection To Myself +90°", "Scene View", modifiers, KeyCode.DownArrow),
                    new Shortcut("Rotate Selection By View Clockwise -90°", "Scene View", modifiers, KeyCode.PageUp),
                    new Shortcut("Rotate Selection By View Counterclockwise +90°", "Scene View", modifiers, KeyCode.PageDown),
                };
#endif
            }
        }
    }
}