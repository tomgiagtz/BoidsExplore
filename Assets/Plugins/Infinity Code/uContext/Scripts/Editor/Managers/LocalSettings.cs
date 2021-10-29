/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext
{
    [Serializable]
    public class LocalSettings : ScriptableObject
    {
        private static LocalSettings _instance;

        [SerializeField]
        private bool _collapseQuickAccessBar = false;

        [SerializeField]
        private bool _enhancedHierarchyShown;

        [SerializeField]
        private bool _hideObjectToolbar;

        [SerializeField]
        private int _upgradeID = 0;

        private static LocalSettings instance
        {
            get
            {
                if (_instance == null) Load();
                return _instance;
            }
        }

        public static bool collapseQuickAccessBar
        {
            get { return instance._collapseQuickAccessBar; }
            set
            {
                instance._collapseQuickAccessBar = value;
                Save();
            }
        }
        

        public static bool enhancedHierarchyShown
        {
            get { return instance._enhancedHierarchyShown; }
            set
            {
                instance._enhancedHierarchyShown = value;
                Save();
            }
        }

        public static bool hideObjectToolbar
        {
            get { return instance._hideObjectToolbar; }
            set
            {
                instance._hideObjectToolbar = value;
                Save();
            }
        }

        public static int upgradeID
        {
            get { return instance._upgradeID; }
            set
            {
                if (_instance._upgradeID >= value) return;

                _instance._upgradeID = value;
                Save();
            }
        }

        private static void Load()
        {
            string path = Resources.assetFolder + "Settings/LocalSettings.asset";
            try
            {
                if (File.Exists(path))
                {
                    try
                    {
                        _instance = AssetDatabase.LoadAssetAtPath<LocalSettings>(path);
                    }
                    catch (Exception e)
                    {
                        Log.Add(e);
                    }

                }

                if (_instance == null)
                {
                    _instance = CreateInstance<LocalSettings>();

#if !UCONTEXT_IGNORE_SETTINGS
                    FileInfo info = new FileInfo(path);
                    if (!info.Directory.Exists) info.Directory.Create();

                    AssetDatabase.CreateAsset(_instance, path);
                    AssetDatabase.SaveAssets();
#endif
                }
            }
            catch (Exception e)
            {
                Log.Add(e);
            }
        }

        public static void Save()
        {
            try
            {
                EditorUtility.SetDirty(_instance);
            }
            catch
            {

            }

        }
    }
}