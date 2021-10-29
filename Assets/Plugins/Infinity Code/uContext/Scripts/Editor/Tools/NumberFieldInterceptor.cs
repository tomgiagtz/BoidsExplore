/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using InfinityCode.uContext.UnityTypes;
using InfinityCode.uContext.Unsafe;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext
{
    [InitializeOnLoad]
    public static class NumberFieldInterceptor
    {
        private static string recycledText;
        internal static readonly string s_AllowedCharactersForFloat = "inftynaeINFTYNAE0123456789.,-*/+%^()";
        internal static readonly string s_AllowedCharactersForInt = "0123456789-*/+%^()";
        private static MemoryPatcher patcher;

        static NumberFieldInterceptor()
        {
            try
            {
                MethodInfo method = typeof(EditorGUI).GetMethod(
                    "DoNumberField", 
                    Reflection.StaticLookup, 
                    null,
                    new []
                    {
                        RecycledTextEditorRef.type,
                        typeof(Rect),
                        typeof(Rect),
                        typeof(int),
                        typeof(bool),
                        typeof(double).MakeByRefType(),
                        typeof(long).MakeByRefType(),
                        typeof(string),
                        typeof(GUIStyle),
                        typeof(bool),
                        typeof(double)
                    }, 
                    null);
                MethodInfo replacement = typeof(NumberFieldInterceptor).GetMethod(
                    "DoNumberField",
                    Reflection.StaticLookup,
                    null,
                    new[]
                    {
                        typeof(object),
                        typeof(Rect),
                        typeof(Rect),
                        typeof(int),
                        typeof(bool),
                        typeof(double).MakeByRefType(),
                        typeof(long).MakeByRefType(),
                        typeof(string),
                        typeof(GUIStyle),
                        typeof(bool),
                        typeof(double)
                    },
                    null);

                patcher = MemoryPatcher.SwapMethods(method, replacement);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal static void DoNumberField(
            object editor,
            Rect position,
            Rect dragHotZone,
            int id,
            bool isDouble,
            ref double doubleVal,
            ref long longVal,
            string formatString,
            GUIStyle style,
            bool draggable,
            double dragSensitivity)
        {
            string allowedChars = isDouble ? s_AllowedCharactersForFloat : s_AllowedCharactersForInt;

            if (draggable)
            {
                EditorGUIRef.DragNumberValue(dragHotZone, id, isDouble, ref doubleVal, ref longVal, dragSensitivity);
            }
            
            Event e = Event.current;
            int v = 0;

            if (Prefs.changeNumberFieldValueByArrow && e.type == EventType.KeyDown && GUIUtility.keyboardControl == id)
            {
                if (e.keyCode == KeyCode.UpArrow)
                {
                    if (e.control || e.command) v = 100;
                    else if (e.shift) v = 10;
                    else v = 1;

                    e.Use();
                }
                else if (e.keyCode == KeyCode.DownArrow)
                {
                    if (e.control || e.command) v = -100;
                    else if (e.shift) v = -10;
                    else v = -1;
                    e.Use();
                }

                if (v != 0)
                {
                    if (isDouble)
                    {
                        if (!double.IsInfinity(doubleVal) && !double.IsNaN(doubleVal))
                        {
                            doubleVal += v;
                            recycledText = doubleVal.ToString(Culture.numberFormat);
                            GUI.changed = true;
                        }
                    }
                    else
                    {
                        longVal += v;
                        recycledText = longVal.ToString();
                        GUI.changed = true;
                    }

                    TextEditorRef.SetText(editor, recycledText);
                    TextEditorRef.SetCursorIndex(editor, 0);
                    TextEditorRef.SetSelectionIndex(editor, recycledText.Length);
                }
            }

            string text;
            if (EditorGUIRef.HasKeyboardFocus(id) || e.type == EventType.MouseDown && e.button == 0 && position.Contains(e.mousePosition))
            {
                if (!RecycledTextEditorRef.IsEditingControl(editor, id))
                {
                    text = recycledText = isDouble ? doubleVal.ToString(formatString, Culture.numberFormat) : longVal.ToString();
                }
                else
                {
                    text = recycledText;
                    if (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed")
                    {
                        text = recycledText = isDouble ? doubleVal.ToString(formatString, Culture.numberFormat) : longVal.ToString();
                    }
                }
            }
            else
            {
                text = isDouble ? doubleVal.ToString(formatString, Culture.numberFormat) : longVal.ToString();
            }

            if (GUIUtility.keyboardControl == id)
            {
                bool changed;
                string str = EditorGUIRef.DoTextField(editor, id, position, text, style, allowedChars, out changed, false, false, false);
                if (!changed) return;
                GUI.changed = true;
                recycledText = str;
                if (isDouble) StringToDouble(str, out doubleVal);
                else StringToLong(str, out longVal);
            }
            else EditorGUIRef.DoTextField(editor, id, position, text, style, allowedChars, out bool _, false, false, false);
        }

        internal static bool StringToDouble(string str, out double value)
        {
            string lower = str.ToLower();
            if (lower == "inf" || lower == "infinity")
                value = double.PositiveInfinity;
            else if (lower == "-inf" || lower == "-infinity")
                value = double.NegativeInfinity;
            else if (lower == "nan")
                value = double.NaN;
            else
                return ExpressionEvaluator.Evaluate<double>(str, out value);
            return false;
        }

        internal static bool StringToLong(string str, out long value)
        {
            return ExpressionEvaluator.Evaluate<long>(str, out value);
        }
    }
}