/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

namespace InfinityCode.uContext.Tools
{
    [InitializeOnLoad]
    public static class HierarchyMainIcon
    {
        private static HashSet<int> hierarchyWindows;
        private static Dictionary<int, CacheItem> bestIconCache;
        private static Texture _unityLogoTexture;
        private static bool inited = false;

        private static Texture unityLogoTexture
        {
            get
            {
                if (_unityLogoTexture == null) _unityLogoTexture = EditorGUIUtility.IconContent("SceneAsset Icon").image;

                return _unityLogoTexture;
            }
        }

        static HierarchyMainIcon()
        {
            hierarchyWindows = new HashSet<int>();
            bestIconCache = new Dictionary<int, CacheItem>();
            HierarchyItemDrawer.Register("HierarchyMainIcon", DrawItem);
        }

        private static void DrawItem(HierarchyItem item)
        {
            if (!Prefs.hierarchyOverrideMainIcon) return;

            if (!inited) Init();

            Event e = Event.current;

            if (e.type == EventType.Layout)
            {
                EditorWindow lastHierarchyWindow = SceneHierarchyWindowRef.GetLastInteractedHierarchy();
                int wid = lastHierarchyWindow.GetInstanceID();
                if (!hierarchyWindows.Contains(wid)) InitWindow(lastHierarchyWindow, wid);
            }

            Texture texture;
            if (!GetTextureFromCache(item, out texture)) return;
            if (texture == null && !InitTexture(item, out texture)) return;

            const int iconSize = 16;

            Rect rect = item.rect;
            Rect iconRect = new Rect(rect) { width = iconSize, height = iconSize };
            iconRect.y += (rect.height - iconSize) / 2;
            GUI.DrawTexture(iconRect, texture, ScaleMode.ScaleToFit);
        }

        private static void GetGameObjectIcon(HierarchyItem item, out Texture texture, GameObject go)
        {
            CacheItem cacheItem = new CacheItem();

            if (PrefabUtility.IsPartOfAnyPrefab(go))
            {
                texture = EditorGUIUtility.IconContent("Prefab Icon").image;
                cacheItem.isPrefab = true;
                return;
            }

            if (go.tag == "Collection") texture = Icons.collection;
            else
            {
                texture = AssetPreview.GetMiniThumbnail(go);

                if (texture.name == "d_GameObject Icon" || texture.name == "GameObject Icon")
                {
                    Component[] components = go.GetComponents<Component>();
                    cacheItem.countComponents = components.Length;

                    if (components.Length > 1)
                    {
                        Component best = components[1];
                        if (components.Length > 2)
                        {
                            if (best is CanvasRenderer)
                            {
                                best = components[2];
                                if (best is Image && components.Length > 3)
                                {
                                    Component c = components[3];
                                    texture = AssetPreview.GetMiniThumbnail(c);
                                    if (texture.name != "cs Script Icon" && texture.name != "d_cs Script Icon") best = c;
                                }
                            }
                        }

                        texture = AssetPreview.GetMiniThumbnail(best);
                    }
                    else texture = null;

                    if (texture == null)
                    {
                        texture = AssetPreview.GetMiniThumbnail(components[0]);
                        if (texture == null) texture = EditorGUIUtility.IconContent("GameObject Icon").image;
                    }
                }
            }

            cacheItem.texture = texture;

            bestIconCache[item.id] = cacheItem;
        }

        private static bool GetTextureFromCache(HierarchyItem item, out Texture texture)
        {
            CacheItem cacheItem;
            texture = null;
            if (!bestIconCache.TryGetValue(item.id, out cacheItem)) return true;

            if (!cacheItem.isPrefab && Event.current.type == EventType.Layout)
            {
                GameObject go = item.gameObject;
                if (go != null)
                {
                    Component[] components = go.GetComponents<Component>();
                    if (components.Length == cacheItem.countComponents) texture = cacheItem.texture;
                }
                else if (item.target == null)
                {
                    texture = unityLogoTexture;
                }
                else return false;
            }
            else texture = cacheItem.texture;

            return true;
        }

        private static bool InitTexture(HierarchyItem item, out Texture texture)
        {
            texture = null;
            if (item.gameObject != null) GetGameObjectIcon(item, out texture, item.gameObject);
            else if (item.target == null)
            {
                CacheItem cacheItem = new CacheItem();
                texture = cacheItem.texture = unityLogoTexture;
                bestIconCache[item.id] = cacheItem;
            }
            else return false;

            return true;
        }

        private static void Init()
        {
            inited = true;
            Object[] windows = UnityEngine.Resources.FindObjectsOfTypeAll(SceneHierarchyWindowRef.type);
            foreach (Object window in windows)
            {
                int wid = window.GetInstanceID();
                if (!hierarchyWindows.Contains(wid)) InitWindow(window as EditorWindow, wid);
            }
        }

        private static void InitWindow(EditorWindow lastHierarchyWindow, int wid)
        {
            if (float.IsNaN(lastHierarchyWindow.rootVisualElement.worldBound.width)) return;

            IMGUIContainer container = lastHierarchyWindow.rootVisualElement.parent.Query<IMGUIContainer>().First();
            container.onGUIHandler = (() => OnGUIBefore(wid)) + container.onGUIHandler;
            HierarchyHelper.SetDefaultIconsSize(lastHierarchyWindow);
            hierarchyWindows.Add(wid);
        }

        private static void OnGUIBefore(int wid)
        {
            if (!Prefs.hierarchyOverrideMainIcon) return;
            if (Event.current.type != EventType.Layout) return;

            List<int> keysForRemove = new List<int>();
            foreach (KeyValuePair<int, CacheItem> pair in bestIconCache)
            {
                if (!pair.Value.used)
                {
                    pair.Value.Dispose();
                    keysForRemove.Add(pair.Key);
                }
            }

            foreach (int key in keysForRemove) bestIconCache.Remove(key);

            foreach (KeyValuePair<int, CacheItem> pair in bestIconCache) pair.Value.used = false;

            EditorWindow w = EditorUtility.InstanceIDToObject(wid) as EditorWindow;
            if (w != null) HierarchyHelper.SetDefaultIconsSize(w);
        }

        internal class CacheItem
        {
            public bool isPrefab;
            public Texture texture;
            public int countComponents;
            public bool used;

            public void Dispose()
            {
                texture = null;
            }
        }
    }
}