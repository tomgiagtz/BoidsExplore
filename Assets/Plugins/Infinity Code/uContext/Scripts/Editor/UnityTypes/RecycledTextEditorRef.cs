/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;

namespace InfinityCode.uContext.UnityTypes
{
    public static class RecycledTextEditorRef
    {
        private static MethodInfo _isEditingControlMethod;
        private static Type _type;

        private static MethodInfo isEditingControlMethod
        {
            get
            {
                if (_isEditingControlMethod == null)
                {
                    _isEditingControlMethod = type.GetMethod(
                        "IsEditingControl",
                        Reflection.InstanceLookup,
                        null,
                        new[] { typeof(int) },
                        null);
                }
                return _isEditingControlMethod;
            }
        }

        public static Type type
        {
            get
            {
                if (_type == null) _type = Reflection.GetEditorType("EditorGUI+RecycledTextEditor");
                return _type;
            }
        }
        public static bool IsEditingControl(object recycledEditor, int id)
        {
            return (bool)isEditingControlMethod.Invoke(recycledEditor, new object[] { id });
        }

    }
}