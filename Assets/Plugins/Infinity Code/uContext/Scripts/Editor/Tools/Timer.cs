/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using InfinityCode.uContext;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.Tools
{
    [InitializeOnLoad]
    public static class Timer
    {
        public static Action OnLeftClick;
        private static GUIContent content;

        static Timer()
        {
            ToolbarManager.AddLeftToolbar("Timer", OnGUI);
        }

        private static void OnGUI()
        {
            if (!EditorApplication.isPlaying ||  !Prefs.timerInToolbar) return;

            float time = Time.time;
            int totalSec = Mathf.FloorToInt(time);
            int hour = totalSec / 3600;
            int min = totalSec / 60 % 60;
            int sec = totalSec % 60;
            int ms = Mathf.RoundToInt((time - (int)time) * 1000);

            float width = 68;

            StaticStringBuilder.Clear();
            if (hour > 0)
            {
                StaticStringBuilder.Append(hour).Append(":");
                width += EditorStyles.textField.CalcSize(TempContent.Get(hour.ToString())).x;
            }
            if (min < 10) StaticStringBuilder.Append("0");
            StaticStringBuilder.Append(min).Append(":");
            if (sec < 10) StaticStringBuilder.Append("0");
            StaticStringBuilder.Append(sec).Append(".");
            if (ms < 100) StaticStringBuilder.Append("0");
            if (ms < 10) StaticStringBuilder.Append("0");
            StaticStringBuilder.Append(ms);

            if (content == null) content = new GUIContent(StaticStringBuilder.GetString(true), "Time since the start of the game.");
            else content.text = StaticStringBuilder.GetString(true);

#if !UNITY_2021_1_OR_NEWER
            GUILayout.BeginVertical();
            GUILayout.Space(5);
#endif

            GUILayout.Label(content, EditorStyles.textField, GUILayout.Width(width));

#if !UNITY_2021_1_OR_NEWER
            GUILayout.EndVertical();
#endif

            Event e = Event.current;
            if (e.type == EventType.Repaint) GUI.changed = true;
            else if (e.type == EventType.MouseDown)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                if (e.button == 0 && rect.Contains(e.mousePosition))
                {
                    if (OnLeftClick != null) OnLeftClick();
                }
            }
        }
    }
}