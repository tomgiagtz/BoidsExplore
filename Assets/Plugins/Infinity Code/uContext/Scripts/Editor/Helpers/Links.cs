/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext
{
    public static class Links
    {
        public const string basic = "https://assetstore.unity.com/packages/tools/level-design/ucontext-basic-182221";
        public const string basicReviews = basic + "/reviews";
        public const string changelog = "https://infinity-code.com/products_update/get-changelog.php?asset=uContext&from=1.0";
        public const string documentation = "https://infinity-code.com/documentation/ucontext.html";
        public const string forum = "https://forum.infinity-code.com";
        public const string homepage = "https://infinity-code.com/assets/ucontext";
        public const string pro = "https://assetstore.unity.com/packages/tools/level-design/ucontext-pro-141831";
        public const string proReviews = pro + "/reviews";
        public const string support = "mailto:support@infinity-code.com?subject=uContext";
        public const string youtube = "https://www.youtube.com/playlist?list=PL2QU1uhBMew_mR83EYhex5q3uZaMTwg1S";

        public static void Open(string url)
        {
            Application.OpenURL(url);
        }

        public static void OpenAssetStore()
        {
#if !UCONTEXT_PRO
            OpenBasic();
#else
            OpenPro();
#endif
        }

        public static void OpenBasic()
        {
            Open(basic);
        }

        public static void OpenBasicReviews()
        {
            Open(basicReviews);
        }

        public static void OpenChangelog()
        {
            Open(changelog);
        }

        [MenuItem(WindowsHelper.MenuPath + "Documentation", false, 120)]
        public static void OpenDocumentation()
        {
            OpenDocumentation(null);
        }

        public static void OpenDocumentation(string anchor)
        {
            string url = documentation;
            if (!string.IsNullOrEmpty(anchor)) url += "#" + anchor;
            Open(url);
        }

        public static void OpenForum()
        {
            Open(forum);
        }

        public static void OpenHomepage()
        {
            Open(homepage);
        }

        public static void OpenLocalDocumentation()
        {
            string url = Resources.assetFolder + "Documentation/Content/Documentation-Content.html";
            Application.OpenURL(url);
        }

        public static void OpenPro()
        {
            Open(pro);
        }

        public static void OpenProReviews()
        {
            Open(proReviews);
        }

        public static void OpenReviews()
        {
#if !UCONTEXT_PRO
            OpenBasicReviews();
#else
            OpenProReviews();
#endif
        }

        public static void OpenSupport()
        {
            Open(support);
        }

        public static void OpenYouTube()
        {
            Open(youtube);
        }
    }
}