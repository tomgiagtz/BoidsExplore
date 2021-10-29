/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext
{
    [InitializeOnLoad]
    public static class HierarchyItemDrawer
    {
        public static Action<HierarchyItem> OnStopped;

        private static List<Listener> listeners;
        private static bool isDirty;
        private static bool isStopped;
        private static HierarchyItem item;

        static HierarchyItemDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
            item = new HierarchyItem();
        }

        private static void OnHierarchyItemGUI(int id, Rect rect)
        {
            if (listeners == null) return;

            if (isDirty)
            {
                listeners.Sort(delegate (Listener i1, Listener i2)
                {
                    if (i1.order == i2.order) return 0;
                    if (i1.order > i2.order) return 1;
                    return -1;
                });

                isDirty = false;
            }

            item.Set(id, rect);

            foreach (Listener listener in listeners)
            {
                if (listener.action != null)
                {
                    try
                    {
                        listener.action(item);
                    }
                    catch (Exception e)
                    {
                        Log.Add(e);
                    }
                }
                if (isStopped) break;
            }

            isStopped = false;
        }

        public static void Register(string id, Action<HierarchyItem> action, float order = 0)
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("ID cannot be empty");

            if (listeners == null) listeners = new List<Listener>();
            int hash = id.GetHashCode();
            foreach (Listener listener in listeners)
            {
                if (listener.hash == hash && listener.id == id)
                {
                    listener.action = action;
                    listener.order = order;
                    return;
                }
            }
            listeners.Add(new Listener
            {
                id = id,
                hash = hash,
                action = action,
                order = order
            });

            isDirty = true;
        }

        public static void StopCurrentRowGUI()
        {
            isStopped = true;
            if (OnStopped != null) OnStopped(item);
        }

        private class Listener
        {
            public int hash;
            public string id;
            public Action<HierarchyItem> action;
            public float order;
        }
    }
}