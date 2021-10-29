/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;

namespace InfinityCode.uContext
{
    [InitializeOnLoad]
    public static class CompilerDefine
    {
        private const string key = "UCONTEXT";

        static CompilerDefine()
        {
            Prefs.InvokeAfterFirstLoad(TryAddSymbols);
            Prefs.ScriptingDefineSymbolsManager.OnAddSymbolsChanged += TryAddSymbols;
        }

        private static void TryAddSymbols()
        {
            if (!Prefs.addScriptingDefineSymbols) return;

            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!string.IsNullOrEmpty(symbols))
            {
                string[] keys = symbols.Split(';');
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] == key) return;
                }
            }

            symbols += ";" + key;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
        }
    }
}