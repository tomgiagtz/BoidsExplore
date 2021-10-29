/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InfinityCode.uContext.JSON;
using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uContext.Windows
{
    [InitializeOnLoad]
    public partial class Bookmarks : EditorWindow
    {
        #region Fields

        public static GUIContent hiddenContent { get; private set; }
        public static GUIContent visibleContent { get; private set; }

        public static Action<Bookmarks> OnToolbarMiddle;

        private static BookmarkItem[] sceneItems;
        private static BookmarkItem[] projectItems;
        private static BookmarkItem[] filteredItems;
        private static BookmarkItem[] folderItems;
        private static List<BookmarkItem> folderItemsStack;
        private static BookmarkItem removeLateItem;

        private static Bookmarks instance;

        private static Texture2D emptyTexture { get; set; }
        
        private bool focusOnSearch;
        private Vector2 scrollPosition;
        private bool showProjectItems = true;
        private bool showSceneItems = true;
        private string _filter = "";
        private static double lastClickTime;

        #endregion

        public static List<BookmarkItem> items
        {
            get
            {
                return ReferenceManager.bookmarks;
            }
        }

        public static JsonArray json
        {
            get
            {
                JsonArray jArr = new JsonArray();
                for (int i = 0; i < items.Count; i++)
                {
                    if (!items[i].isProjectItem) continue;
                    jArr.Add(items[i].json);
                }

                return jArr;
            }
            set
            {
                if (value.count == 0) return;

                if (items.Any(i => i.isProjectItem))
                {
                    // 0 - Replace, 1 - Ignore, 2 - Append
                    int action = EditorUtility.DisplayDialogComplex("Import Bookmarks", "Bookmarks already contain project items", "Replace", "Ignore", "Append");
                    if (action == 1) return;
                    if (action == 0) items.RemoveAll(i => i.isProjectItem);
                }
                
                foreach (JsonItem v in value)
                {
                    string path = v.V<string>("path");
                    if (!File.Exists(path)) continue;
                    if (items.Any(i => i.isProjectItem && i.path == path)) continue;

                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    BookmarkItem item = new BookmarkItem(obj);
                    items.Add(item);
                }

                Redraw();
            }
        }

        public string filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                UpdateFilteredItems();
            }
        }

        static Bookmarks()
        {
            KeyManager.KeyBinding binding = KeyManager.AddBinding();
            binding.OnValidate += OnValidate;
            binding.OnInvoke += () => ShowWindow();
        }

        public static void Add(Object target) 
        {
            int id = target.GetInstanceID();
            string gid = GlobalObjectId.GetGlobalObjectIdSlow(target).ToString();
            if (items.Any(i => i.globalInstanceID == gid || i.temporaryID == id)) return;

            BookmarkItem item = new BookmarkItem(target);
            items.Add(item);
            Save();
            if (instance != null) UpdateItemsGroup();
        }

        private void BottomBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            gridSize = (int)GUILayout.HorizontalSlider(gridSize, minGridSize, maxGridSize, GUILayout.Width(100));
            if (EditorGUI.EndChangeCheck()) EditorPrefs.SetInt(GridSizePref, gridSize);
            EditorGUILayout.EndHorizontal();
        }

        private static void ClearFilter()
        {
            instance._filter = "";
            filteredItems = null;

            TextEditorRef.SetText(string.Empty);
        }

        public static bool Contain(Object item)
        {
            int id = item.GetInstanceID();
            string gid = GlobalObjectId.GetGlobalObjectIdSlow(item).ToString();
            return items.Any(i => i.globalInstanceID == gid || i.temporaryID == id);
        }

        private static void DisposeFolderItems()
        {
            if (folderItems == null) return;

            foreach (BookmarkItem item in folderItems) item.Dispose();
            folderItems = null;
        }

        private void DrawItems(ref BookmarkItem removeItem)
        {
            if (filteredItems != null)
            {
                if (gridSize > minGridSize) DrawGridItems(filteredItems, ref removeItem);
                else DrawTreeItems(filteredItems, ref removeItem);
            }
            else if (folderItems != null)
            {
                if (gridSize > minGridSize) DrawGridItems(folderItems, ref removeItem);
                else DrawTreeItems(folderItems, ref removeItem);
            }
            else
            {
                if (sceneItems == null || projectItems == null) UpdateItemsGroup();

                if (sceneItems.Length > 0)
                {
                    showSceneItems = GUILayout.Toggle(showSceneItems, "Scene Items", EditorStyles.foldoutHeader);
                    if (showSceneItems)
                    {
                        if (gridSize > minGridSize) DrawGridItems(sceneItems, ref removeItem);
                        else DrawTreeItems(sceneItems, ref removeItem);
                    }
                }

                if (projectItems.Length > 0)
                {
                    showProjectItems = GUILayout.Toggle(showProjectItems, "Project Items", EditorStyles.foldoutHeader);
                    if (showProjectItems)
                    {
                        if (gridSize > minGridSize) DrawGridItems(projectItems, ref removeItem);
                        else DrawTreeItems(projectItems, ref removeItem);
                    }
                }
            }
        }

        private void FolderItemsToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            BookmarkItem current = folderItemsStack.Last();

            if (folderItemsStack.Count > 1)
            {
                if (GUILayoutUtils.ToolbarButton(EditorIconContents.animationFirstKey))
                {
                    ClearFilter();

                    DisposeFolderItems();
                    folderItemsStack.Clear();
                    folderItems = null;
                    return;
                }
            }

            if (GUILayoutUtils.ToolbarButton(EditorIconContents.animationPrevKey))
            {
                ClearFilter();

                folderItemsStack.RemoveAt(folderItemsStack.Count - 1);
                if (folderItemsStack.Count > 0) SelectParentFolder(folderItemsStack.Last());
                else DisposeFolderItems();
            }

            EditorGUILayout.LabelField(current.title);
            EditorGUILayout.EndHorizontal();
        }

        private static void InitFolderItems(BookmarkItem folderItem)
        {
            DisposeFolderItems();

            IEnumerable<string> entries = Directory.GetFileSystemEntries(folderItem.path);
            List<BookmarkItem> tempItems = new List<BookmarkItem>();
            foreach (string entry in entries)
            {
                if (entry.EndsWith(".meta")) continue;

                Object asset = AssetDatabase.LoadAssetAtPath<Object>(entry);
                if (asset == null) continue;

                BookmarkItem item = new BookmarkItem(asset);
                tempItems.Add(item);
            }

            folderItems = tempItems.ToArray();
        }

        private void InitPreview(BookmarkItem item)
        {
            if (item.isMissed || item.target == null)
            {
                item.preview = emptyTexture;
                item.previewLoaded = true;
            }
            else if (item.isProjectItem && item.target is GameObject)
            {
                bool isLoading = AssetPreview.IsLoadingAssetPreview(item.target.GetInstanceID());
                if (isLoading)
                {
                    item.preview = EditorResources.prefabTexture;
                }
                else
                {
                    item.preview = AssetPreview.GetAssetPreview(item.target);
                    if (item.preview == null) item.preview = AssetPreview.GetMiniThumbnail(item.target);
                    item.previewLoaded = true;
                }
            }
            else
            {
                item.preview = AssetPreview.GetMiniThumbnail(item.target);
                item.previewLoaded = true;
            }
        }

        public static void InsertBookmarkMenu(GenericMenuEx menu, Object target)
        {
            if (Contain(target)) menu.Add("Remove Bookmark", () => Remove(target));
            else menu.Add("Add Bookmark", () => Add(target));
        }

        private void OnDestroy()
        {
            if (folderItems != null)
            {
                foreach (BookmarkItem item in folderItems) item.Dispose();
                folderItems = null;
            }

            folderItemsStack = null;

            instance = null;
        }

        private void OnEnable()
        {
            instance = this;
            minSize = new Vector2(250, 250);

            gridSize = EditorPrefs.GetInt(GridSizePref, 16);

            showContent = new GUIContent(Styles.isProSkin? Icons.openNewWhite: Icons.openNewBlack, "Show");
            closeContent = new GUIContent(Styles.isProSkin ? Icons.closeWhite: Icons.closeBlack, "Remove");

            hiddenContent = EditorIconContents.sceneVisibilityHiddenHover;
            visibleContent = EditorIconContents.sceneVisibilityVisibleHover;
        }

        private void OnGUI()
        {
            if (instance == null) instance = this;
            if (emptyTexture == null) emptyTexture = Resources.CreateSinglePixelTexture(1f, 0f);
            if (showContentStyle == null || showContentStyle.normal.background == null)
            {
                showContentStyle = new GUIStyle(Styles.transparentButton);
                showContentStyle.margin.top = 6;
                showContentStyle.margin.left = 6;
            }
            if (selectedStyle == null)
            {
                selectedStyle = new GUIStyle(Styles.selectedRow);
                selectedStyle.fixedHeight = 0;
            }

            if (removeLateItem != null)
            {
                Remove(removeLateItem.target);
                removeLateItem = null;
            }

            ProcessEvents();
            Toolbar();

            if (folderItemsStack != null && folderItemsStack.Count > 0)
            {
                FolderItemsToolbar();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            BookmarkItem removeItem = null;
            DrawItems(ref removeItem);

            if (removeItem != null)
            {
                items.Remove(removeItem);
                Save();
                UpdateItemsGroup();
                UpdateFilteredItems();
            }

            EditorGUILayout.EndScrollView();

            BottomBar();
        }

        private static bool OnValidate()
        {
            return Prefs.bookmarksHotKey && Event.current.modifiers == Prefs.bookmarksModifiers && Event.current.keyCode == Prefs.bookmarksKeyCode;
        }

        private void ProcessEvents()
        {
            if (mouseOverWindow != this) return;
            if (folderItems != null) return;

            Event e = Event.current;
            if (e.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                e.Use();
            }
            else if (e.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                e.Use();

                foreach (Object obj in DragAndDrop.objectReferences) Add(obj);
            }
            else if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.F && (e.modifiers == EventModifiers.Control || e.modifiers == EventModifiers.Command))
                {
                    focusOnSearch = true;
                    e.Use();
                }
            }
        }

        private void ProcessDoubleClick(BookmarkItem item)
        {
            if (EditorApplication.timeSinceStartup - lastClickTime > 0.3)
            {
                lastClickTime = EditorApplication.timeSinceStartup;
                return;
            }

            lastClickTime = 0;

            if (item.target is AudioClip)
            {
                AudioClip audioClip = item.target as AudioClip;
                if (AudioUtilsRef.IsClipPlaying(audioClip)) AudioUtilsRef.StopClip(audioClip);
                else AudioUtilsRef.PlayClip(audioClip);
            }
            else if (item.target is DefaultAsset)
            {
                FileAttributes attributes = File.GetAttributes(item.path);
                if ((attributes & FileAttributes.Directory) == FileAttributes.Directory) SelectFolder(item);
            }
            else if (item.target is Component)
            {
                ComponentWindow.Show(item.target as Component);
            }
            else if (item.target is GameObject)
            {
                if (PrefabUtility.IsPartOfAnyPrefab(item.gameObject))
                {
                    GameObjectUtils.OpenPrefab(item.path);
                }
            }
            else EditorUtility.OpenWithDefaultApp(item.path);
        }

        public static void Redraw()
        {
            if (instance != null) instance.Repaint();
            UpdateItemsGroup();
        }

        private static void RefreshItems()
        {
            foreach (BookmarkItem item in items) item.isMissed = false;
        }

        public static void Remove(Object item)
        {
            int id = item.GetInstanceID();
            string gid = GlobalObjectId.GetGlobalObjectIdSlow(item).ToString();
            List<BookmarkItem> records = items;
            for (int i = records.Count - 1; i >= 0; i--)
            {
                BookmarkItem r = records[i];
                if (r.globalInstanceID == gid || r.temporaryID == id) records.RemoveAt(i);
            }
            
            Save();
            if (instance != null)
            {
                UpdateItemsGroup();
                instance.UpdateFilteredItems();
            }
        }

        private static void RemoveLate(BookmarkItem item)
        {
            removeLateItem = item;
        }

        private static void Save()
        {
            ReferenceManager.Save();
        }

        private static void SelectFolder(BookmarkItem folderItem)
        {
            ClearFilter();

            InitFolderItems(folderItem);

            if (folderItemsStack == null) folderItemsStack = new List<BookmarkItem>();
            folderItemsStack.Add(folderItem);
        }

        private void SelectParentFolder(BookmarkItem folderItem)
        {
            InitFolderItems(folderItem);
        }

        private void ShowContextMenu(BookmarkItem item)
        {
            if (item.target is Component)
            {
                ComponentUtils.ShowContextMenu(item.target as Component);
            }
            else if (item.target is GameObject)
            {
                if (!item.isProjectItem) GameObjectUtils.ShowContextMenu(false, item.target as GameObject);
                else ShowOtherContextMenu(item);
            }
            else ShowOtherContextMenu(item);
        }

        private void ShowOtherContextMenu(BookmarkItem item)
        {
            GenericMenuEx menu = GenericMenuEx.Start();
            menu.Add("Remove Bookmark", () => RemoveLate(item));
            menu.ShowAsContext();
        }

        public static EditorWindow ShowDropDownWindow(Vector2? mousePosition = null)
        {
            if (!mousePosition.HasValue) mousePosition = Event.current.mousePosition;
            Bookmarks wnd = CreateInstance<Bookmarks>();
            wnd.titleContent = new GUIContent("Bookmarks");
            Vector2 position = GUIUtility.GUIToScreenPoint(mousePosition.Value);
            Vector2 size = Prefs.defaultWindowSize;
            Rect rect = new Rect(position - size / 2, size);
            if (rect.y < 30) rect.y = 30;

            wnd.position = rect;
            wnd.ShowPopup();
            wnd.Focus();

            PinAndClose.Show(wnd, rect, wnd.Close, () =>
            {
                Rect wRect = wnd.position;
                wRect.yMin -= PinAndClose.HEIGHT;
                ShowWindow().position = wRect;
                wnd.Close();
            }, "Bookmarks");
            return wnd;
        }

        public static EditorWindow ShowWindow(Vector2? mousePosition)
        {
            Bookmarks wnd = CreateInstance<Bookmarks>();
            wnd.titleContent = new GUIContent("Bookmarks");
            wnd.Show();

            Vector2? mp = null;
            if (mousePosition.HasValue) mp = mousePosition.Value;
            else if (Event.current != null) mp = Event.current.mousePosition;

            if (mp.HasValue)
            {
                Vector2 screenPoint = GUIUtility.GUIToScreenPoint(mp.Value);
                Vector2 size = Prefs.defaultWindowSize;
                wnd.position = new Rect(screenPoint - size / 2, size);
            }
            return wnd;
        }

        [MenuItem(WindowsHelper.MenuPath + "Bookmarks", false, 100)]
        public static EditorWindow ShowWindow()
        {
            return ShowWindow(null);
        }

        public static EditorWindow ShowUtilityWindow(Vector2? mousePosition = null)
        {
            if (!mousePosition.HasValue) mousePosition = Event.current.mousePosition;
            Bookmarks wnd = CreateInstance<Bookmarks>();
            wnd.titleContent = new GUIContent("Bookmarks");
            wnd.ShowUtility();
            wnd.Focus();
            Vector2 size = Prefs.defaultWindowSize;
            wnd.position = new Rect(GUIUtility.GUIToScreenPoint(mousePosition.Value) - size / 2, size);
            return wnd;
        }

        private void Toolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("uContextBookmarkSearchTextField");
            _filter = EditorGUILayoutEx.ToolbarSearchField(_filter);

            if (focusOnSearch && Event.current.type == EventType.Repaint)
            {
                GUI.FocusControl("uContextBookmarkSearchTextField");
                focusOnSearch = false;
            }

            if (EditorGUI.EndChangeCheck()) UpdateFilteredItems();

            if (OnToolbarMiddle != null) OnToolbarMiddle(this);

            if (GUILayoutUtils.ToolbarButton("Refresh")) RefreshItems();
            if (GUILayoutUtils.ToolbarButton("Clear"))
            {
                if (EditorUtility.DisplayDialog("Clear Bookmarks", "Do you really want to clear your bookmarks?", "Clear", "Cancel"))
                {
                    items.Clear();
                    Save();
                    _filter = string.Empty;
                    UpdateItemsGroup(); 
                }
            }

            if (GUILayoutUtils.ToolbarButton("?")) Links.OpenDocumentation("bookmarks");

            EditorGUILayout.EndHorizontal();
        }

        private void UpdateFilteredItems()
        {
            if (string.IsNullOrEmpty(_filter))
            {
                filteredItems = null;
                return;
            }

            string assetType;
            string pattern = SearchableItem.GetPattern(_filter, out assetType);

            IEnumerable<BookmarkItem> targetItems;
            if (folderItems == null) targetItems = items;
            else targetItems = folderItems;

            filteredItems = targetItems.Where(i => i.Update(pattern, assetType) > 0).OrderByDescending(i => i.accuracy).ToArray();
        }

        private static void UpdateItemsGroup()
        {
            sceneItems = items.Where(i => !i.isProjectItem).OrderBy(i => i.title).ToArray();
            projectItems = items.Where(i => i.isProjectItem).OrderBy(i => i.title).ToArray();
        }
    }
}