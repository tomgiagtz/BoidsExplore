/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.uContext.Windows;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.Editors
{
    [CustomEditor(typeof(Documentation))]
    public class DocumentationEditor: Editor
    {
        private static void DrawDocumentation()
        {
            if (GUILayout.Button("Local Documentation"))
            {
                Links.OpenLocalDocumentation();
            }

            if (GUILayout.Button("Online Documentation"))
            {
                Links.OpenDocumentation();
            }

            GUILayout.Space(10);
        }

        private static void DrawExtra()
        {
            if (GUILayout.Button("Getting Started"))
            {
                GettingStarted.OpenWindow();
            }

#if !UCONTEXT_PRO
            if (GUILayout.Button("Basic vs Pro"))
            {
                Links.OpenDocumentation("basic-vs-pro");
            }
#endif


            if (GUILayout.Button("Changelog"))
            {
                Links.OpenChangelog();
            }

            if (GUILayout.Button("Shortcuts"))
            {
                Shortcuts.OpenWindow();
            }

            if (GUILayout.Button("Settings"))
            {
                Settings.OpenSettings();
            }

            GUILayout.Space(10);
        }

        private new static void DrawHeader()
        {
#if UCONTEXT_PRO
            GUILayout.Label("uContext Pro", Styles.centeredLabel);
#else
            GUILayout.Label("uContext Basic", Styles.centeredLabel);
#endif
            GUILayout.Label("version: " + uContextMenu.version, Styles.centeredLabel);
            GUILayout.Space(10);
        }

        private void DrawRateAndReview()
        {
            EditorGUILayout.HelpBox("Please don't forget to leave a review on the store page if you liked uContext, this helps us a lot!", MessageType.Warning);

            if (GUILayout.Button("Rate & Review"))
            {
                Links.OpenReviews();
            }
        }

        private void DrawSupport()
        {
            if (GUILayout.Button("Support"))
            {
                Links.OpenSupport();
            }

            if (GUILayout.Button("Forum"))
            {
                Links.OpenForum();
            }

            GUILayout.Space(10);
        }

        public override void OnInspectorGUI()
        {
            DrawHeader();
            DrawDocumentation();
            DrawExtra();
            DrawSupport();
            DrawRateAndReview();
        }
    }
}
