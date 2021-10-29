/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace InfinityCode.uContext.UnityTypes
{
    public static class SceneHierarchyRef
    {
        private static MethodInfo _setScenesExpandedMethod;
        private static FieldInfo _treeViewField;
        private static Type _type;

        private static MethodInfo setScenesExpandedMethod
        {
            get
            {
                if (_setScenesExpandedMethod == null) _setScenesExpandedMethod = type.GetMethod("SetScenesExpanded", Reflection.InstanceLookup, null, new[] {typeof(List<string>)}, null);
                return _setScenesExpandedMethod;
            }
        }

        private static FieldInfo treeViewField
        {
            get
            {
                if (_treeViewField == null) _treeViewField = type.GetField("m_TreeView", Reflection.InstanceLookup);
                return _treeViewField;
            }
        }

        public static Type type
        {
            get
            {
                if (_type == null) _type = Reflection.GetEditorType("SceneHierarchy");
                return _type;
            }
        }

        public static object GetTreeView(object instance)
        {
            return treeViewField.GetValue(instance);
        }

        public static void SetScenesExpanded(object instance, List<string> scenes)
        {
            setScenesExpandedMethod.Invoke(instance, new object[] {scenes});
        }
    }
}