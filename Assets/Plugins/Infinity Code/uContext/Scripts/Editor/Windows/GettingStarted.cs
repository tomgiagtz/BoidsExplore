/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.IO;
using System.Text;
using InfinityCode.uContext.JSON;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.Windows
{
    public class GettingStarted : EditorWindow
    {
        private static string folder;
        private Slide[] slides;
        private int totalSlides;
        private Slide activeSlide;
        private Slide first;
        private Slide last;
        private Rect buttonsRect;

        [NonSerialized]
        private int mode = 0; // 0 - slides, 1 - table of contents 

        private GUIContent[] contents;
        private GUIContent tableContent = new GUIContent("≡", "Table of Contents");
        private GUIContent helpContent = new GUIContent("?", "Open Documentation");

        [NonSerialized]
        private Vector2 scrollPosition;

        [NonSerialized]
        private GUIStyle _style;

        private GUIStyle style
        {
            get
            {
                if (_style == null)
                {
                    _style = new GUIStyle(EditorStyles.label);
                    _style.padding.left = 5;
                }

                return _style;
            }
        }

        private void DrawActiveSlide()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Space || e.keyCode == KeyCode.RightArrow)
                {
                    SetSlide(activeSlide.next);
                }
                else if (e.keyCode == KeyCode.Backspace || e.keyCode == KeyCode.LeftArrow)
                {
                    SetSlide(activeSlide.prev);
                }

                UpdateTitle();
                Repaint();
            }
            else if (e.type == EventType.MouseUp && !buttonsRect.Contains(e.mousePosition))
            {
                if (e.button == 0)
                {
                    SetSlide(activeSlide.next);
                }
                else if (e.button == 1)
                {
                    SetSlide(activeSlide.prev);
                }

                UpdateTitle();

                Repaint();
            }

            if (activeSlide.texture != null) GUI.DrawTexture(new Rect(2, 2, position.width - 4, position.height - 4), activeSlide.texture);
            int ti = GUI.Toolbar(buttonsRect, -1, contents);
            if (ti != -1)
            {
                if (ti == 0) mode = 1;
                else if (ti == 1) Links.OpenDocumentation(activeSlide.help);
            }
        }

        private void DrawTableOfContent()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUILayout.Space(10);
            for (int i = 0; i < slides.Length; i++) DrawTableOfContentSlide(slides[i], 0);
            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            GUI.changed = true;
        }

        private void DrawTableOfContentSlide(Slide slide, int indent)
        {
            Rect r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label, GUILayout.Height(18));

            Event e = Event.current;
            if (e.type == EventType.Repaint)
            {
                if (r.Contains(e.mousePosition))
                {
                    GUI.DrawTexture(r, Styles.selectedRowTexture);
                }
                int cid = GUIUtility.GetControlID("TOCItem".GetHashCode(), FocusType.Passive);
                Rect r2 = new Rect(r);
                r2.x += indent * 20;
                style.Draw(r2, TempContent.Get(slide.title), cid);
            }
            else if (e.type == EventType.MouseUp && r.Contains(e.mousePosition))
            {
                SetSlide(slide);
                UpdateTitle();
                mode = 0;
                e.Use();
            }

            if (slide.slides != null)
            {
                for (int i = 0; i < slide.slides.Length; i++) DrawTableOfContentSlide(slide.slides[i], indent + 1);
            }
        }

        private void InitSlides(Slide[] slides, ref int index, ref Slide prev)
        {
            for (int i = 0; i < slides.Length; i++)
            {
                Slide slide = slides[i];
                if (!string.IsNullOrEmpty(slide.image))
                {
                    slide.prev = prev;
                    slide.index = ++index;
                    if (prev == null) activeSlide = slide;
                    else prev.next = slide;
                    prev = slide;
                }

                if (slide.slides != null) InitSlides(slide.slides, ref index, ref prev);
            }
        }

        private void OnDisable()
        {
            if (slides != null)
            {
                foreach (Slide slide in slides) slide.Dispose();
                slides = null;
            }

            first = null;
            last = null;
            activeSlide = null;
        }

        private void OnEnable()
        {
            folder = Resources.assetFolder + "Textures/Getting Started/";
            string content = File.ReadAllText(folder + "Content.json", Encoding.UTF8);

            slides = Json.Deserialize<Slide[]>(content);

            Slide prev = null;
            totalSlides = 0;
            InitSlides(slides, ref totalSlides, ref prev);
            
            last = prev;
            first = activeSlide;

            first.prev = last;
            last.next = first;

            minSize = new Vector2(604, 454);
            maxSize = new Vector2(604, 454);

            UpdateTitle();
            SetSlide(activeSlide);
        }

        public void OnGUI()
        {
            if (mode == 0) DrawActiveSlide();
            else DrawTableOfContent();
        }

        [MenuItem(WindowsHelper.MenuPath + "Getting Started", false, 121)]
        public static void OpenWindow()
        {
            GettingStarted wnd = GetWindow<GettingStarted>(true, "Getting Started", true);
            wnd.SetSlide(wnd.activeSlide); 
            wnd.UpdateTitle();
        }

        private void SetSlide(Slide slide)
        {
            if (string.IsNullOrEmpty(slide.image))
            {
                if (slide.slides == null) return;
                
                bool success = false;
                for (int i = 0; i < slide.slides.Length; i++)
                {
                    Slide s = slide.slides[i];
                    if (string.IsNullOrEmpty(s.image)) continue;

                    slide = s;
                    success = true;
                    break;
                }

                if (!success) return;
            }
            activeSlide = slide;
            if (string.IsNullOrEmpty(slide.help))
            {
                buttonsRect = new Rect(position.width - 35, 5, 30, 20);
                contents = new[] { tableContent };
            }
            else
            {
                buttonsRect = new Rect(position.width - 55, 5, 50, 20);
                contents = new[] { tableContent, helpContent };
            }
        }

        private void UpdateTitle()
        {
            titleContent = new GUIContent("Getting Started. Frame " + activeSlide.index + " / " + totalSlides + " (click to continue)");
        }

        public class Slide
        {
            public string title;
            public string image;
            public string help;
            public Slide[] slides;
            public int index;

            public Slide next;
            public Slide prev;
            private Texture2D _texture;

            public Texture2D texture
            {
                get
                {
                    if (_texture == null)
                    {
                        _texture = AssetDatabase.LoadAssetAtPath<Texture2D>(folder + image);
                    }

                    return _texture;
                }
            }

            public void Dispose()
            {
                if (slides != null)
                {
                    foreach (Slide slide in slides)
                    {
                        slide.Dispose();
                    }
                }

                slides = null;
                next = null;
                prev = null;
                _texture = null;
            }
        }
    }
}