/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.Tools
{
    [InitializeOnLoad]
    public static class HierarchyHeader
    {
        private static GUIStyle _headerStyle;
        private static char[] _trimChars;

        private static GUIStyle headerStyle
        {
            get
            {
                if (_headerStyle == null)
                {
                    _headerStyle = new GUIStyle();
                    Color color = Prefs.hierarchyHeaderBackground;
                    color.a = 1;
                    _headerStyle.normal.background = Resources.CreateSinglePixelTexture(color);
                    _headerStyle.normal.textColor = Prefs.hierarchyHeaderTextColor;
                    _headerStyle.alignment = TextAnchor.MiddleCenter;
                    _headerStyle.fontStyle = FontStyle.Bold;
                }
                else if (_headerStyle.normal.background == null)
                {
                    Color color = Prefs.hierarchyHeaderBackground;
                    color.a = 1;
                    _headerStyle.normal.background = Resources.CreateSinglePixelTexture(color);
                }

                return _headerStyle;
            }
        }

        private static char[] trimChars
        {
            get
            {
                if (_trimChars == null)
                {
                    _trimChars = Prefs.hierarchyHeaderTrimChars.ToCharArray();
                }

                return _trimChars;
            }
        }

        static HierarchyHeader()
        {
            HierarchyItemDrawer.Register("HierarchyHeader", OnHierarchyItem, -10000);
        }

        [MenuItem("GameObject/Create Header", priority = 1)]
        public static GameObject Create()
        {
            GameObject go = new GameObject(Prefs.hierarchyHeaderPrefix + "Header");
            go.tag = "EditorOnly";
            GameObject active = Selection.activeGameObject;
            if (active != null)
            {
                go.transform.SetParent(active.transform.parent);
                go.transform.SetSiblingIndex(active.transform.GetSiblingIndex());
            }
            Undo.RegisterCreatedObjectUndo(go, go.name);
            Selection.activeGameObject = go;
            return go;
        }

        private static void OnHierarchyItem(HierarchyItem item)
        {
            if (!Prefs.hierarchyHeaders) return;

            GameObject go = item.gameObject;
            if (go == null) return;

            string name = go.name;
            string prefix = Prefs.hierarchyHeaderPrefix;

            if (name.Length < prefix.Length) return;
            for (int i = 0; i < prefix.Length; i++)
            {
                if (name[i] != prefix[i]) return;
            }

            if (Event.current.type == EventType.Repaint)
            {
                Rect rect = item.rect;
                Rect r = new Rect(32, rect.y, rect.xMax - 16, rect.height);

                int start = prefix.Length;
                int end = name.Length;

                for (int i = start; i < name.Length; i++)
                {
                    char c = name[i];
                    int j;
                    for (j = 0; j < trimChars.Length; j++)
                    {
                        if (trimChars[j] == c)
                        {
                            start++;
                            break;
                        }
                    }
                    if (j == trimChars.Length) break;
                }

                for (int i = end - 1; i > start; i--)
                {
                    char c = name[i];
                    int j;
                    for (j = 0; j < trimChars.Length; j++)
                    {
                        if (trimChars[j] == c)
                        {
                            end--;
                            break;
                        }
                    }
                    if (j == trimChars.Length) break;
                }

                name = name.Substring(start, end - start);

                headerStyle.Draw(r, TempContent.Get(name), 0, false, false);
            }

            HierarchyItemDrawer.StopCurrentRowGUI();
        }

        public static void ResetStyle()
        {
            _headerStyle = null;
        }

        public static void ResetTrimChars()
        {
            _trimChars = null;
        }
    }
}