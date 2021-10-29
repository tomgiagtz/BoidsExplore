/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uContext.UnityTypes
{
    public static class EditorGUIUtilityRef
    {
        private static MethodInfo _drawEditorHeaderItemsMethod;
        private static FieldInfo _lastControlIDField;

        private static MethodInfo drawEditorHeaderItemsMethod
        {
            get
            {
                if (_drawEditorHeaderItemsMethod == null)
                {
                    _drawEditorHeaderItemsMethod = Reflection.GetMethod(type, "DrawEditorHeaderItems", new[] { typeof(Rect), typeof(Object[]), typeof(float) }, Reflection.StaticLookup);
                }

                return _drawEditorHeaderItemsMethod;
            }
        }

        private static FieldInfo lastControlIDField
        {
            get
            {
                if (_lastControlIDField == null) _lastControlIDField = type.GetField("s_LastControlID", Reflection.StaticLookup);
                return _lastControlIDField;
            }
        }

        public static Type type
        {
            get { return typeof(EditorGUIUtility); }
        }

        public static void DrawEditorHeaderItems(Rect rect, Object[] objects, int id)
        {
            drawEditorHeaderItemsMethod.Invoke(null, new object[] { rect, objects, id });
        }

        public static int GetLastControlID()
        {
            return (int)lastControlIDField.GetValue(null);
        }
    }
}