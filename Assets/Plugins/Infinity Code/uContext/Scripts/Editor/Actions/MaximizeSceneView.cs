/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.uContext.Integration;
using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.Actions
{
    public class MaximizeSceneView : ActionItem, IValidatableLayoutItem
    {
        public override float order
        {
            get { return 900; }
        }

        protected override void Init()
        {
            bool maximized;
            if (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType() == GameViewRef.type) maximized = WindowsHelper.IsMaximized(EditorWindow.focusedWindow);
            else maximized = WindowsHelper.IsMaximized(SceneView.lastActiveSceneView);

            if (maximized)
            {
                guiContent = new GUIContent(Icons.minimize, "Minimize Active Window");
            }
            else
            {
                string tooltip = "Maximize Active Window";
                if (FullscreenEditor.isPresent) tooltip += "\nUse SHIFT to switch the window in full screen.";
                guiContent = new GUIContent(Icons.maximize, tooltip);
            }
        }

        public override void Invoke()
        {
            try
            {
                if (uContextMenu.lastWindow != null && uContextMenu.lastWindow.GetType() == GameViewRef.type)
                {
                    if (Event.current.shift && FullscreenEditor.isPresent && !WindowsHelper.IsMaximized(uContextMenu.lastWindow)) FullscreenEditor.MakeFullscreen(uContextMenu.lastWindow);
                    else WindowsHelper.ToggleMaximized(uContextMenu.lastWindow);
                }
                else
                {
                    if (Event.current.shift && FullscreenEditor.isPresent && !WindowsHelper.IsMaximized(SceneView.lastActiveSceneView)) FullscreenEditor.MakeFullscreen(SceneView.lastActiveSceneView);
                    else WindowsHelper.ToggleMaximized(SceneView.lastActiveSceneView);
                }
            }
            catch
            {
                
            }
        }

        public bool Validate()
        {
            return SceneView.lastActiveSceneView != null || (EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.GetType() == GameViewRef.type);
        }
    }
}