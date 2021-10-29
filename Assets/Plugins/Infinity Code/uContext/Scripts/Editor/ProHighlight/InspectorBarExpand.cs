/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.uContext.Inspector;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.ProHighlight
{
#if !UCONTEXT_PRO
    [InitializeOnLoad]
    public static class InspectorBarExpand
    {
        private static GUIContent _collapseContent;

        private static GUIContent collapseContent
        {
            get
            {
                if (_collapseContent == null) _collapseContent = new GUIContent(Icons.collapse, "Collapse all components (Available in uContext PRO)");
                return _collapseContent;
            }
        }

        static InspectorBarExpand()
        {
            InspectorBar.OnDrawBefore += DrawExpand;
        }

        private static void DrawExpand(EditorWindow wnd, Editor[] editors)
        {
            if (!Prefs.proHighlight) return;
            if (!GUILayout.Button(collapseContent, EditorStyles.toolbarButton, GUILayout.Width(25))) return;

            if (EditorUtility.DisplayDialog("Warning", "Collapse/expand all components available in uContext Pro.", "Open uContext Pro", "Cancel"))
            {
                Links.OpenPro();
            }
        }
    }

#endif
}