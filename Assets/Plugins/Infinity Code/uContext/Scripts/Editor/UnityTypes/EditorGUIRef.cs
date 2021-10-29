/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.UnityTypes
{
    public static class EditorGUIRef
    {
        private static MethodInfo _dragNumberValueMethod;
        private static MethodInfo _hasKeyboardFocusMethod;
        private static FieldInfo _recycledEditorField;
        private static MethodInfo _doTextFieldMethod;

        private static MethodInfo doTextFieldMethod
        {
            get
            {
                if (_doTextFieldMethod == null)
                {
                    _doTextFieldMethod = type.GetMethod(
                        "DoTextField",
                        Reflection.StaticLookup,
                        null,
                        new[]
                        {
                            RecycledTextEditorRef.type,
                            typeof(int),
                            typeof(Rect),
                            typeof(string),
                            typeof(GUIStyle),
                            typeof(string),
                            typeof(bool).MakeByRefType(),
                            typeof(bool),
                            typeof(bool),
                            typeof(bool)
                        }, 
                        null
                    );
                }
                return _doTextFieldMethod;
            }
        }

        private static MethodInfo dragNumberValueMethod
        {
            get
            {
                if (_dragNumberValueMethod == null)
                {
                    _dragNumberValueMethod = type.GetMethod(
                        "DragNumberValue",
                        Reflection.StaticLookup,
                        null,
                        new[]
                        {
                            typeof(Rect),
                            typeof(int),
                            typeof(bool),
                            typeof(double).MakeByRefType(),
                            typeof(long).MakeByRefType(),
                            typeof(double)
                        }, 
                        null
                    );
                }
                return _dragNumberValueMethod;
            }
        }

        private static MethodInfo hasKeyboardFocusMethod
        {
            get
            {
                if (_hasKeyboardFocusMethod == null)
                {
                    _hasKeyboardFocusMethod = type.GetMethod(
                        "HasKeyboardFocus", 
                        Reflection.StaticLookup,
                        null,
                        new []{typeof(int)},
                        null);
                }
                return _hasKeyboardFocusMethod;
            }
        }

        private static FieldInfo recycledEditorField
        {
            get
            {
                if (_recycledEditorField == null) _recycledEditorField = type.GetField("s_RecycledEditor", Reflection.StaticLookup);
                return _recycledEditorField;
            }
        }

        public static Type type
        {
            get { return typeof(EditorGUI); }
        }

        public static string DoTextField(
            object editor,
            int id,
            Rect position,
            string text,
            GUIStyle style,
            string allowedletters,
            out bool changed,
            bool reset,
            bool multiline,
            bool passwordField)
        {
            object[] args = new object[]
            {
                editor,
                id,
                position,
                text,
                style,
                allowedletters,
                null,
                reset,
                multiline,
                passwordField
            };
            string result = (string) doTextFieldMethod.Invoke(null, args);
            changed = (bool)args[6];
            return result;
        }

        public static void DragNumberValue(Rect dragHotZone,
            int id,
            bool isDouble,
            ref double doubleVal,
            ref long longVal,
            double dragSensitivity)
        {
            object[] args = new object[]
            {
                dragHotZone,
                id,
                isDouble,
                doubleVal,
                longVal,
                dragSensitivity
            };
            dragNumberValueMethod.Invoke(null, args);
            doubleVal = (double)args[3];
            longVal = (long)args[4];
        }

        public static object GetRecycledEditor()
        {
            return recycledEditorField.GetValue(null);
        }

        public static bool HasKeyboardFocus(int id)
        {
            return (bool) hasKeyboardFocusMethod.Invoke(null, new object[] {id});
        }
    }
}