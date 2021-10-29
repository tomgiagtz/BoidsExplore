/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditorInternal;

namespace InfinityCode.uContext.UnityTypes
{
    public static class ReorderableListRef
    {
        public static Type type
        {
            get { return typeof(ReorderableList); }
        }

#if UNITY_2020_2_OR_NEWER
        private static MethodInfo _clearCacheMethod;

        private static MethodInfo clearCacheMethod
        {
            get
            {
                if (_clearCacheMethod == null) _clearCacheMethod = type.GetMethod("ClearCache", Reflection.InstanceLookup);
                return _clearCacheMethod;
            }
        }
#endif

        public static void ClearCache(ReorderableList list)
        {
#if UNITY_2020_2_OR_NEWER
            clearCacheMethod.Invoke(list, null);
#endif
        }
    }
}