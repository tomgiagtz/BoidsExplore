using InfinityCode.uContext.Inspector;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.ProHighlight
{
#if !UCONTEXT_PRO
    [InitializeOnLoad]
    public static class NestedEditor
    {
#if UNITY_2021_1_OR_NEWER
        private const bool TOGGLE_ON_LABEL_CLICK = false;
#else
        private const bool TOGGLE_ON_LABEL_CLICK = true;
#endif

        private static GUIStyle _backgroundStyle;

        private static GUIStyle backgroundStyle
        {
            get
            {
                if (_backgroundStyle == null)
                {
                    _backgroundStyle = new GUIStyle(GUI.skin.box)
                    {
                        margin =
                        {
                            top = 0,
                            bottom = 0
                        }
                    };
                }

                return _backgroundStyle;
            }
        }

        static NestedEditor()
        {
            ObjectFieldDrawer.OnGUIAfter += OnGUIAfter;
        }

        private static void OnGUIAfter(Rect area, SerializedProperty property, GUIContent label)
        {
            if (!Prefs.proHighlight || !Prefs.nestedEditors) return;

            property.isExpanded = EditorGUI.Foldout(area, property.isExpanded, GUIContent.none, TOGGLE_ON_LABEL_CLICK);
            if (!property.isExpanded) return;

            GUILayout.BeginVertical(backgroundStyle);

            GUILayout.Label("Nested Editors are available in uContext Pro.");
            if (GUILayout.Button("Open uContext Pro")) Links.OpenPro();

            GUILayout.EndVertical();
        }
    }
#endif
}