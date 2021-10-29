/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEngine;

namespace InfinityCode.uContext.UnityTypes
{
    public static class GUIViewRef
    {
        private static Type _type;

        public static Type type
        {
            get
            {
                if (_type == null) _type = Reflection.GetEditorType("GUIView");
                return _type;
            }
        }

#if !UNITY_2020_1_OR_NEWER
        private static PropertyInfo _visualTreeProp;

        public static PropertyInfo visualTreeProp
        {
            get
            {
                if (_visualTreeProp == null) _visualTreeProp = type.GetProperty("visualTree", Reflection.InstanceLookup);
                return _visualTreeProp;
            }
        }
#endif

#if UNITY_2020_1_OR_NEWER
        private static PropertyInfo _windowBackendProp;

        public static PropertyInfo windowBackendProp
        {
            get
            {
                if (_windowBackendProp == null) _windowBackendProp = type.GetProperty("windowBackend", Reflection.InstanceLookup);
                return _windowBackendProp;
            }
        }
#endif
    }
}