/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.Integration
{
    [InitializeOnLoad]
    public static class FullscreenEditor
    {
        private static MethodInfo getFullscreenFromViewMethod;
        private static MethodInfo toggleFullscreenMethod;
        private static MethodInfo makeFullscreenMethod;

        public static bool isPresent { get; }

        static FullscreenEditor()
        {
            Assembly assembly = Reflection.GetAssembly("FullscreenEditor");
            if (assembly == null) return;

            Type feType = assembly.GetType("FullscreenEditor.Fullscreen");
            if (feType == null) return;

            getFullscreenFromViewMethod = Reflection.GetMethod(feType, "GetFullscreenFromView", new[] {typeof(ScriptableObject), typeof(bool)}, BindingFlags.Static | BindingFlags.Public);
            toggleFullscreenMethod = Reflection.GetMethod(feType, "ToggleFullscreen", new[] {typeof(ScriptableObject)}, BindingFlags.Static | BindingFlags.Public);
            makeFullscreenMethod = Reflection.GetMethod(feType, "MakeFullscreen", new[] {typeof(Type), typeof(EditorWindow), typeof(bool)}, BindingFlags.Static | BindingFlags.Public);

            if (getFullscreenFromViewMethod == null || toggleFullscreenMethod == null || makeFullscreenMethod == null) return;

            isPresent = true;
        }

        public static object GetFullscreenFromView(ScriptableObject viewOrWindow, bool rootView = true)
        {
            if (!isPresent) return null;
            return getFullscreenFromViewMethod.Invoke(null, new object[] {viewOrWindow, rootView});
        }

        public static bool IsFullscreen(ScriptableObject viewOrWindow)
        {
            return GetFullscreenFromView(viewOrWindow) != null;
        }

        public static object MakeFullscreen(EditorWindow window)
        {
            return MakeFullscreen(typeof(EditorWindow), window);
        }

        public static object MakeFullscreen(Type type, EditorWindow window = null, bool disposableWindow = false)
        {
            if (!isPresent) return null;
            return makeFullscreenMethod.Invoke(null, new object[] {type, window, disposableWindow});
        }

        public static void ToggleFullscreen(ScriptableObject viewOrWindow)
        {
            if (!isPresent) return;
            toggleFullscreenMethod.Invoke(null, new[] { viewOrWindow });
        }
    }
}