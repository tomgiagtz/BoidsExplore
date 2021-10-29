/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using System.Linq;

namespace InfinityCode.uContext
{
    public static partial class Prefs
    {
        public class SceneViewManager : StandalonePrefManager<SceneViewManager>
        {
            public override IEnumerable<string> keywords
            {
                get
                {
                    return ObjectToolbarManager.GetKeywords()
                        .Concat(DuplicateToolManager.GetKeywords())
                        .Concat(DistanceToolManager.GetKeywords())
                        .Concat(TerrainBrushSizeManager.GetKeywords())
                        .Concat(ToolValuesManager.GetKeywords());
                }
            }

            public override void Draw()
            {
                DuplicateToolManager.Draw(null);
                ObjectToolbarManager.Draw(null);
                TerrainBrushSizeManager.Draw(null);
                DistanceToolManager.Draw(null);
                ToolValuesManager.Draw(null);
            }
        }
    }
}