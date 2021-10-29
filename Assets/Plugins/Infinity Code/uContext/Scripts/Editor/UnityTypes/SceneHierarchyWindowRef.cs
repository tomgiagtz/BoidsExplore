using System;
using System.Reflection;
using UnityEditor;

namespace InfinityCode.uContext.UnityTypes
{
    public static class SceneHierarchyWindowRef
    {
        private static MethodInfo _setExpandedRecursiveMethod;
        private static FieldInfo _sceneHierarchyField;
        private static FieldInfo _lastInteractedHierarchyField;
        private static Type _type;

        private static MethodInfo setExpandedRecursiveMethod
        {
            get
            {
                if (_setExpandedRecursiveMethod == null) _setExpandedRecursiveMethod = type.GetMethod("SetExpandedRecursive", Reflection.InstanceLookup, null, new[] { typeof(int), typeof(bool) }, null);
                return _setExpandedRecursiveMethod;
            }
        }

        private static FieldInfo sceneHierarchyField
        {
            get
            {
                if (_sceneHierarchyField == null) _sceneHierarchyField = type.GetField("m_SceneHierarchy", Reflection.InstanceLookup);
                return _sceneHierarchyField;
            }
        }

        private static FieldInfo lastInteractedHierarchyField
        {
            get
            {
                if (_lastInteractedHierarchyField == null) _lastInteractedHierarchyField = type.GetField("s_LastInteractedHierarchy", Reflection.StaticLookup);
                return _lastInteractedHierarchyField;
            }
        }

        public static Type type
        {
            get
            {
                if (_type == null) _type = Reflection.GetEditorType("SceneHierarchyWindow");
                return _type;
            }
        }

        public static EditorWindow GetLastInteractedHierarchy()
        {
            return lastInteractedHierarchyField.GetValue(null) as EditorWindow;
        }

        public static object GetSceneHierarchy(object instance)
        {
            return sceneHierarchyField.GetValue(instance);
        }

        public static void SetExpandedRecursive(object instance, int id, bool expand)
        {
            setExpandedRecursiveMethod.Invoke(instance, new object[] {id, expand});
        }
    }
}