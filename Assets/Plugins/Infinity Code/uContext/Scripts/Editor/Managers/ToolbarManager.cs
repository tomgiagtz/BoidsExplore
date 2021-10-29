/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace InfinityCode.uContext
{
    [InitializeOnLoad]
    public static class ToolbarManager
    {
        private const float space = 10;
        private const float largeSpace = 20;
        private const float buttonWidth = 32;
        private const float dropdownWidth = 80;
        private const float playPauseStopWidth = 140;

        private static int toolCount;
        private static GUIStyle style;

        private static Dictionary<string, Action> leftToolbarItems;
        private static Dictionary<string, Action> rightToolbarItems;

        private static ScriptableObject currentToolbar;

        static ToolbarManager()
        {
            toolCount = ToolbarRef.GetToolCount();
            EditorApplication.update -= CheckCurrentToolbar;
            EditorApplication.update += CheckCurrentToolbar;
        }

        public static void AddLeftToolbar(string key, Action action, int order = 0)
        {
            if (leftToolbarItems == null) leftToolbarItems = new Dictionary<string, Action>();
            leftToolbarItems[key] = action;
        }

        public static void AddRightToolbar(string key, Action action, int order = 0)
        {
            if (rightToolbarItems == null) rightToolbarItems = new Dictionary<string, Action>();
            rightToolbarItems[key] = action;
        }

        private static void CheckCurrentToolbar()
        {
            if (currentToolbar != null) return;

            Object[] toolbars = UnityEngine.Resources.FindObjectsOfTypeAll(ToolbarRef.type);
            if (toolbars.Length == 0)
            {
                currentToolbar = null;
                return;
            }

            currentToolbar = (ScriptableObject) toolbars[0];
            if (currentToolbar == null) return;

#if UNITY_2021_1_OR_NEWER
            VisualElement root = ToolbarRef.GetRoot(currentToolbar);

            CreateArea(root, "ToolbarZoneLeftAlign", Justify.FlexEnd, DrawLeftToolbarItems);
            CreateArea(root, "ToolbarZoneRightAlign", Justify.FlexStart, DrawRightToolbarItems);
#else

            VisualElement visualTree = Compatibility.GetVisualTree(currentToolbar);
            IMGUIContainer container = (IMGUIContainer)visualTree[0];

            Action handler = IMGUIContainerRef.GetGUIHandler(container);
            handler -= OnGUI;
            handler += OnGUI;
            IMGUIContainerRef.SetGUIHandler(container, handler);
#endif
        }

        private static void CreateArea(VisualElement root, string zoneName, Justify justify, Action action)
        {
            if (action == null) return;

            VisualElement toolbar = root.Q(zoneName);
            VisualElement parent = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                    justifyContent = justify
                }
            };

            IMGUIContainer container = new IMGUIContainer();
            container.onGUIHandler += action.Invoke;
            parent.Add(container);
            toolbar.Add(parent);
        }

        private static void DrawLeftToolbar(float screenWidth, float playButtonsPosition)
        {
            if (leftToolbarItems == null || leftToolbarItems.Count == 0) return;

            Rect rect = new Rect(0, 0, screenWidth, Screen.height);
            rect.xMin += space * 2 + buttonWidth * toolCount + largeSpace + 128;
            rect.xMax = playButtonsPosition - space;
            rect.y = 1;
            rect.height = 24;

            if (rect.width <= 0) return;

            GUILayout.BeginArea(rect);
            DrawLeftToolbarItems();
            GUILayout.EndArea();
        }

        private static void DrawLeftToolbarItems()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            foreach (var pair in leftToolbarItems)
            {
                try
                {
                    if (pair.Value != null) pair.Value();
                }
                catch (Exception e)
                {
                    Log.Add(e);
                }
            }

            GUILayout.EndHorizontal();
        }

        private static void DrawRightToolbar(float screenWidth, float playButtonsPosition)
        {
            if (rightToolbarItems == null || rightToolbarItems.Count == 0) return;

            Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition + style.fixedWidth * 3 + space;
            rightRect.xMax = screenWidth - space * 5 - dropdownWidth * 3 - largeSpace - buttonWidth - 78;
            rightRect.y = 4;
            rightRect.height = 24;

            if (rightRect.width <= 0) return;

            GUILayout.BeginArea(rightRect);
            DrawRightToolbarItems();
            GUILayout.EndArea();
        }

        private static void DrawRightToolbarItems()
        {
            GUILayout.BeginHorizontal();

            foreach (var pair in rightToolbarItems)
            {
                try
                {
                    if (pair.Value != null) pair.Value();
                }
                catch (Exception e)
                {
                    Log.Add(e);
                }
            }

            GUILayout.EndHorizontal();
        }

        private static void OnGUI()
        {
            if (style == null) style = new GUIStyle("CommandLeft");

            float screenWidth = EditorGUIUtility.currentViewWidth;
            float playButtonsPosition = Mathf.RoundToInt((screenWidth - playPauseStopWidth) / 2);

            DrawLeftToolbar(screenWidth, playButtonsPosition);
            DrawRightToolbar(screenWidth, playButtonsPosition);
        }

        public static bool RemoveLeftToolbar(string key)
        {
            if (leftToolbarItems == null) return false;
            return leftToolbarItems.Remove(key);
        }

        public static bool RemoveRightToolbar(string key)
        {
            if (rightToolbarItems == null) return false;
            return rightToolbarItems.Remove(key);
        }
    }
}