/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.uContext.Integration;
using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.Tools
{
    [InitializeOnLoad]
    public static class MaximizeGameView
    {
        static MaximizeGameView()
        {
            KeyManager.KeyBinding binding = KeyManager.AddBinding();
            binding.OnValidate += () => Prefs.improveMaximizeGameViewBehaviour;
            binding.OnInvoke += OnInvoke;
        }

        private static void OnInvoke()
        {
            Event e = Event.current;
            EditorWindow wnd = EditorWindow.focusedWindow;
            if (wnd != null && 
                ((wnd.GetType() == GameViewRef.type && e.keyCode == KeyCode.Space && e.modifiers == EventModifiers.Shift) || 
                 (!FullscreenEditor.isPresent && e.keyCode == KeyCode.F11 && e.modifiers == EventModifiers.FunctionKey)))
            {
                WindowsHelper.SetMaximized(wnd, !wnd.maximized);
                Event.current.Use();
            }
        }
    }
}