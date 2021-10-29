/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections;
using InfinityCode.uContext.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfinityCode.uContext.UnityTypes
{
    [HideInIntegrity]
    public static class Compatibility
    {
        public static IList GetGameViews()
        {
            return PlayModeViewRef.GetPlayModeViews();
        }

        public static VisualElement GetVisualTree(ScriptableObject scriptableObject)
        {
#if UNITY_2020_1_OR_NEWER
            object backend = GUIViewRef.windowBackendProp.GetValue(scriptableObject, null);
            return (VisualElement)IWindowBackendRef.visualTreeProp.GetValue(backend, null);
#else
            return (VisualElement)GUIViewRef.visualTreeProp.GetValue(scriptableObject, null);
#endif
        }
    }
}