/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.uContext.Tools;
using UnityEditor;

namespace InfinityCode.uContext.ProHighlight
{
#if !UCONTEXT_PRO
    [InitializeOnLoad]
    public static class OpenTimescale
    {

        static OpenTimescale()
        {
            Timer.OnLeftClick += OnTimerClick;
        }

        private static void OnTimerClick()
        {
            if (!Prefs.proHighlight) return;

            if (EditorUtility.DisplayDialog("Warning", "The ability to change the timescale available in uContext Pro.", "Open uContext Pro", "Cancel"))
            {
                Links.OpenPro();
            }
        }
    }

#endif
}