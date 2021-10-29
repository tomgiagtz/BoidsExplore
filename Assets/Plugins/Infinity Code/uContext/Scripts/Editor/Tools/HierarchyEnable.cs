/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.Tools
{
    [InitializeOnLoad]
    public static class HierarchyEnable
    {
        static HierarchyEnable()
        {
            HierarchyItemDrawer.Register("HierarchyEnable", OnHierarchyItem, -1);
        }

        private static void OnHierarchyItem(HierarchyItem item)
        {
            if (!Prefs.hierarchyEnableGameObject) return;
            if (item.gameObject == null || !item.hovered) return;

            Event e = Event.current;
            if (Prefs.hierarchyIconsDisplayRule == HierarchyIconsDisplayRule.onHoverWithModifiers && e.modifiers != Prefs.hierarchyIconsModifiers) return;

            Rect rect = item.rect;
            Rect r = new Rect(32, rect.y, 16, rect.height);

            EditorGUI.BeginChangeCheck();
            bool v = EditorGUI.Toggle(r, GUIContent.none, item.gameObject.activeSelf);
            if (EditorGUI.EndChangeCheck())
            {
                item.gameObject.SetActive(v);
                EditorUtility.SetDirty(item.gameObject);
            }
        }
    }
}