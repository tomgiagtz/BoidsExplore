/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using UnityEditor;

namespace InfinityCode.uContext
{
    public static partial class Prefs
    {
        public static bool inspectorBar = true;
        public static bool nestedEditors = true;
        public static bool dragObjectFields = true;

        public class InspectorManager : StandalonePrefManager<InspectorManager>
        {
            public override IEnumerable<string> keywords
            {
                get
                {
                    return new[]
                    {
                        "Inspector Bar",
                        "Nested Editor",
                        "Drag Object Field"
                    };
                }
            }

            public override void Draw()
            {
                dragObjectFields = EditorGUILayout.ToggleLeft("Drag Object Fields", dragObjectFields);
                inspectorBar = EditorGUILayout.ToggleLeft("Inspector Bar", inspectorBar);
                nestedEditors = EditorGUILayout.ToggleLeft("Nested Editors", nestedEditors);
            }
        }
    }
}