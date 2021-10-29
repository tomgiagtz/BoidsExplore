/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEngine;

namespace InfinityCode.uContext.UnityTypes
{
    public static class TextEditorRef
    {
        private static FieldInfo _cursorIndexField;
        private static FieldInfo _selectIndexField;
        private static PropertyInfo _textProp;

        private static FieldInfo cursorIndexField
        {
            get
            {
                if (_cursorIndexField == null) _cursorIndexField = type.GetField("m_CursorIndex", Reflection.InstanceLookup);
                return _cursorIndexField;
            }
        }

        private static FieldInfo selectIndexField
        {
            get
            {
                if (_selectIndexField == null) _selectIndexField = type.GetField("m_SelectIndex", Reflection.InstanceLookup);
                return _selectIndexField;
            }
        }

        private static PropertyInfo textProp
        {
            get
            {
                if (_textProp == null) _textProp = type.GetProperty("text", Reflection.InstanceLookup);
                return _textProp;
            }
        }

        public static Type type
        {
            get { return typeof(TextEditor); }
        }

        public static void SetCursorIndex(object recycledEditor, int index)
        {
            cursorIndexField.SetValue(recycledEditor, index);
        }

        public static void SetSelectionIndex(object recycledEditor, int index)
        {
            selectIndexField.SetValue(recycledEditor, index);
        }

        public static void SetText(string text)
        {
            SetText(EditorGUIRef.GetRecycledEditor(), text);
        }

        public static void SetText(object recycledEditor, string text)
        {
            textProp.SetValue(recycledEditor, text);
        }
    }
}