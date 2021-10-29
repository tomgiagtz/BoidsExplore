/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using InfinityCode.uContext.Actions;
using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace InfinityCode.uContext.Inspector
{
    [InitializeOnLoad]
    public static class InspectorBar
    {
        public static Action<EditorWindow, Editor[]> OnDrawBefore;
        public static Action<EditorWindow, Editor[]> OnDrawAfter;

        private const string ELEMENT_NAME = "InspectorBar";
        private static GUIStyle _selectedContentStyle;
        private static Dictionary<int, ContentWrapper> contantCache;

        private static GUIStyle selectedContentStyle
        {
            get
            {
                if (_selectedContentStyle == null || _selectedContentStyle.normal.background == null)
                {
                    GUIStyle s = EditorStyles.toolbarButton;
                    _selectedContentStyle = new GUIStyle
                    {
                        normal =
                        {
                            background = Resources.CreateSinglePixelTexture(1, 0.1f),
                            textColor = s.normal.textColor
                        },
                        margin = s.margin,
                        padding = s.padding,
                        fixedHeight = s.fixedHeight,
                        alignment = s.alignment
                    };
                }

                return _selectedContentStyle;
            }
        }

        static InspectorBar()
        {
            EditorApplication.delayCall += InitInspector;
            WindowManager.OnWindowFocused += OnWindowFocused;
            WindowManager.OnMaximizedChanged += OnMaximizedChanged;
            Selection.selectionChanged += OnSelectionChanged;
            contantCache = new Dictionary<int, ContentWrapper>();
        }

        private static void DrawBar(EditorWindow wnd, VisualElement editorsList)
        {
            if (editorsList == null) return;
            if (editorsList.childCount < 1) return;
            VisualElement elements = editorsList[0];

            Editor[] editors = EditorElementRef.GetEditors(elements);
            if (editors == null || editors.Length < 2) return;

            Event e = Event.current;
            GUIStyle normalStyle = EditorStyles.toolbarButton;

            EditorGUILayout.BeginHorizontal();

            if (OnDrawBefore != null) OnDrawBefore(wnd, editors);

            int editorIndex = 0;

            for (int i = 0; i < editorsList.childCount; i++)
            {
                VisualElement el = editorsList[i];
                if (el.GetType().Name != "EditorElement") continue;
                if (el.childCount < 2)
                {
                    editorIndex++;
                    continue;
                }

                Editor editor = editors[editorIndex];
                if (editor == null || editor.target == null) continue;

                int id = editor.target.GetInstanceID();
                ContentWrapper wrapper;

                if (!contantCache.TryGetValue(id, out wrapper)) wrapper = InitContent(editor, normalStyle, id);

                StyleEnum<DisplayStyle> display = el.style.display;
                bool isActive = display.keyword == StyleKeyword.Null || display == DisplayStyle.Flex;
                GUIStyle style = isActive ? normalStyle : selectedContentStyle;

                ButtonEvent buttonEvent = GUILayoutUtils.Button(wrapper.content, style, GUILayout.Width(wrapper.width));
                if (buttonEvent == ButtonEvent.hover)
                {
                    wnd.Focus();
                }
                else if (buttonEvent == ButtonEvent.click)
                {
                    if (e.button == 0)
                    {
                        if (e.command || e.control || e.shift) ToggleVisible(wnd, editorsList, i, editorIndex, !isActive);
                        else
                        {
                            if (!isActive) SetSoloVisible(wnd, editorsList, i, editorIndex, false);
                            else
                            {
                                int countActive = 0;
                                for (int j = 0; j < editorsList.childCount; j++)
                                {
                                    VisualElement el2 = editorsList[j];
                                    if (el2.childCount < 2) continue;
                                    display = el2.style.display;
                                    if (display.keyword == StyleKeyword.Null || display == DisplayStyle.Flex) countActive++;
                                }
                                SetSoloVisible(wnd, editorsList, i, editorIndex, countActive == 1);
                            }
                        }
                    }
                    else if (e.button == 1) ComponentUtils.ShowContextMenu(editor.target);
                    e.Use();
                }
                else if (buttonEvent == ButtonEvent.drag)
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.objectReferences = editor.targets;
                    DragAndDrop.StartDrag("Drag " + editor.target.name);
                    e.Use();
                }

                editorIndex++;
            }

            GUIContent addContentContent = TempContent.Get("+", "Add Component");

            if (GUILayoutUtils.ToolbarButton(addContentContent))
            {
                Vector2 s = Prefs.defaultWindowSize;
                Rect wp = wnd.position;
                Rect r = GUILayoutUtility.GetLastRect();
                Vector2 p = GUIUtility.GUIToScreenPoint(r.position);
                p.x = wp.x + (wp.width - s.x) / 2;
                p.y += 45;
                Rect rect = new Rect(p, s);

                AddComponent.ShowAddComponent(rect);
            }

            GUILayout.FlexibleSpace();

            if (OnDrawAfter != null) OnDrawAfter(wnd, editors);

            if (GUILayoutUtils.ToolbarButton("?")) Links.OpenDocumentation("inspector-bar");

            EditorGUILayout.EndHorizontal();
        }

        private static ContentWrapper InitContent(Editor editor, GUIStyle normalStyle, int id)
        {
            Texture thumbnail = AssetPreview.GetMiniThumbnail(editor.target);
            string tooltip = ObjectNames.NicifyVariableName(editor.target.GetType().Name);

            ContentWrapper wrapper = new ContentWrapper
            {
                content = new GUIContent(thumbnail, tooltip)
            };

            bool useIcon = true;
            if (thumbnail.name == "cs Script Icon" || thumbnail.name == "d_cs Script Icon")
            {
                GameObjectUtils.GetPsIconContent(wrapper.content);
                useIcon = false;
            }

            Vector2 s = new Vector2();

            if (!useIcon)
            {
                s = normalStyle.CalcSize(wrapper.content);
                if (s.x < 25) s.x = 25;
            }
            else s.x = 25;

            wrapper.width = s.x;
            contantCache.Add(id, wrapper);
            return wrapper;
        }

        private static void InitInspector()
        {
            Object[] windows = UnityEngine.Resources.FindObjectsOfTypeAll(InspectorWindowRef.type);
            List<EditorWindow> failedWindows = new List<EditorWindow>();
            foreach (EditorWindow wnd in windows)
            {
                if (wnd == null) continue;
                if (!InjectBar(wnd)) failedWindows.Add(wnd);
            }

            if (failedWindows.Count > 0)
            {
                SceneViewManager.OnNextGUI += () => TryReinit(failedWindows);
            }
        }

        private static VisualElement GetMainContainer(EditorWindow wnd)
        {
            return wnd != null ? GetVisualElement(wnd.rootVisualElement, "unity-inspector-main-container") : null;
        }

        private static VisualElement GetVisualElement(VisualElement element, string className)
        {
            for (int i = 0; i < element.childCount; i++)
            {
                VisualElement el = element[i];
                if (el.ClassListContains(className)) return el;
                el = GetVisualElement(el, className);
                if (el != null) return el;
            }

            return null;
        }

        private static bool InjectBar(EditorWindow wnd)
        {
            if (!Prefs.inspectorBar) return false;

            VisualElement mainContainer = GetMainContainer(wnd);
            if (mainContainer == null) return false;
            if (mainContainer.childCount < 2) return false;

            if (mainContainer[0].name == ELEMENT_NAME) mainContainer.RemoveAt(0);

            VisualElement editorsList = GetVisualElement(mainContainer, "unity-inspector-editors-list");
            if (editorsList.childCount < 2) return false;
            VisualElement elements = editorsList[0];

            Editor[] editors = EditorElementRef.GetEditors(elements);
            if (editors == null || editors.Length < 2) return false;
            Object target = editors[0].target;

            if (!(target is GameObject) && target.GetType() != PrefabImporterRef.type) return false;

            VisualElement element = new IMGUIContainer(() => DrawBar(wnd, editorsList));
            element.name = ELEMENT_NAME;
            element.style.height = 20;
            element.style.position = Position.Relative;
            mainContainer.Insert(0, element);

            return true;
        }

        private static void OnMaximizedChanged(EditorWindow w)
        {
            Object[] windows = UnityEngine.Resources.FindObjectsOfTypeAll(InspectorWindowRef.type);
            foreach (EditorWindow wnd in windows)
            {
                if (wnd == null) continue;

                InjectBar(wnd);
            }
        }

        private static void OnSelectionChanged()
        {
            contantCache.Clear();
            InitInspector();
        }

        private static void OnWindowFocused(EditorWindow wnd)
        {
            if (wnd == null) return;
            if (wnd.GetType() != InspectorWindowRef.type) return;
            InjectBar(wnd);
        }

        private static bool IsTransform(VisualElement el)
        {
            return el.name == "Transform" || el.name == "Rect Transform";
        }

        private static void SetSoloVisible(EditorWindow wnd, VisualElement element, int index, int editorIndex, bool show)
        {
            if (show)
            {
                for (int i = 0; i < element.childCount; i++)
                {
                    VisualElement el = element[i];
                    el.style.display = DisplayStyle.Flex;
                    if (IsTransform(el)) el.style.marginTop = 0;
                }
            }
            else
            {
                for (int i = 0; i < element.childCount; i++)
                {
                    VisualElement el = element[i];
                    if (i == index)
                    {
                        el.style.display = DisplayStyle.Flex;
                        if (IsTransform(el)) el.style.marginTop = 7;
                        object inspectorElement = EditorElementRef.GetInspectorElement(el);
                        EditorElementRef.SetElementVisible(inspectorElement, false);

                    }
                    else el.style.display = DisplayStyle.None;
                }

                ActiveEditorTracker tracker = InspectorWindowRef.GetTracker(wnd);
                tracker.SetVisible(editorIndex, 1);
            }
        }

        private static void ToggleVisible(EditorWindow wnd, VisualElement element, int index, int editorIndex, bool show)
        {
            VisualElement el = element[index];
            if (show)
            {
                el.style.display = DisplayStyle.Flex;
                if (IsTransform(el)) el.style.marginTop = 0;
            }
            else
            {
                el.style.display = DisplayStyle.None;
                for (int i = 0; i < element.childCount; i++)
                {
                    el = element[i];
                    if (el.childCount < 2) continue;

                    if (IsTransform(el))
                    {
                        el.style.marginTop = 7;
                        break;
                    }
                    if (el.style.display == DisplayStyle.Flex) break;
                }
                ActiveEditorTracker tracker = InspectorWindowRef.GetTracker(wnd);
                tracker.SetVisible(editorIndex, 1);
            }
        }

        private static void TryReinit(List<EditorWindow> windows)
        {
            foreach (EditorWindow wnd in windows)
            {
                if (wnd == null) continue;
                InjectBar(wnd);
            }
        }

        internal class ContentWrapper
        {
            public GUIContent content;
            public float width;
        }
    }
}