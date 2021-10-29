﻿/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using InfinityCode.uContext.JSON;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uContext.Windows
{
    [Serializable]
    public class BookmarkItem : SearchableItem
    {
        public string title;
        public string globalInstanceID;
        public string type;
        public string path;
        public Object target;
        public bool isProjectItem;

        [NonSerialized]
        public string tooltip;

        [NonSerialized]
        public int temporaryID = int.MaxValue;

        [NonSerialized]
        public bool isMissed = false;

        [NonSerialized]
        public Texture preview;

        private string[] _search;
        
        [NonSerialized]
        public bool previewLoaded;

        public GameObject gameObject
        {
            get
            {
                if (target == null) return null;
                if (target is Component) return (target as Component).gameObject;
                if (target is GameObject) return target as GameObject;
                return null;
            }
        }

        public JsonObject json
        {
            get
            {
                JsonObject obj = new JsonObject();
                obj.Add("path", path, JsonValue.ValueType.STRING);
                return obj;
            }
        }

        public BookmarkItem()
        {

        }

        public BookmarkItem(Object obj)
        {
            target = obj;

            globalInstanceID = GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString();
            Type t = obj.GetType();
            type = t.AssemblyQualifiedName;

            if (obj is GameObject)
            {
                GameObject go = obj as GameObject;
                title = go.name;
                isProjectItem = go.scene.name == null;
                tooltip = GameObjectUtils.GetTransformPath(go.transform).ToString();

                if (!isProjectItem) path = "/" + tooltip;
                else path = AssetDatabase.GetAssetOrScenePath(go);
            }
            else if (obj is Component)
            {
                GameObject go = (obj as Component).gameObject;
                title = go.name + " (" + ObjectNames.NicifyVariableName(t.Name) + ")";
                tooltip = GameObjectUtils.GetTransformPath(go.transform).ToString();

                isProjectItem = go.scene.name == null;

                if (!isProjectItem) path = "/" + tooltip;
                else path = AssetDatabase.GetAssetPath(go);
            }
            else
            {
                title = obj.name;
                int instanceID = obj.GetInstanceID();
                tooltip = AssetDatabase.GetAssetPath(instanceID);
                path = AssetDatabase.GetAssetOrScenePath(obj);
                isProjectItem = true;
            }
        }

        public void Dispose()
        {
            target = null;
            preview = null;
            _search = null;
        }

        protected override string[] GetSearchStrings()
        {
            if (_search == null) _search = new[] { title };

            return _search;
        }

        public void TryRestoreTarget()
        {
            GlobalObjectId gid;
            if (GlobalObjectId.TryParse(globalInstanceID, out gid))
            {
                Object obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
                if (obj != null && obj.GetType().AssemblyQualifiedName == type) target = obj;
            }

            isMissed = target == null;
        }

        public float Update(string pattern, string valueType)
        {
            _accuracy = 0;

            if (!string.IsNullOrEmpty(valueType) && !Contains(type, valueType)) return 0;
            return UpdateAccuracy(pattern);
        }
    }
}