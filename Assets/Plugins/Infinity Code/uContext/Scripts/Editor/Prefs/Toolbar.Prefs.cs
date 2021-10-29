/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace InfinityCode.uContext
{
    public static partial class Prefs
    {
        public static bool timerInToolbar = true;
        public static bool showViewStateToolbarIcon = true;

        public class ToolbarManager : StandalonePrefManager<ToolbarManager>
        {
            public override IEnumerable<string> keywords
            {
                get
                {
                    return ToolbarWindowsManager.GetKeywords().Concat(new []
                    {
                        "Show icon on toolbar if selection has View State",
                        "Timer"
                    });
                }
            }

            public override void Draw()
            {
                showViewStateToolbarIcon = EditorGUILayout.ToggleLeft("Show Icon If Selection Has View State", showViewStateToolbarIcon);
                timerInToolbar = EditorGUILayout.ToggleLeft("Timer", timerInToolbar);
                ToolbarWindowsManager.Draw(null);
            }
        }
    }
}