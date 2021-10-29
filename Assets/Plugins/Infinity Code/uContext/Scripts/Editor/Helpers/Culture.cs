/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Globalization;

namespace InfinityCode.uContext
{
    public static class Culture
    {
        public static CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        public static NumberFormatInfo numberFormat = cultureInfo.NumberFormat;
        public static TextInfo textInfo = cultureInfo.TextInfo;
        public static CompareInfo compareInfo = cultureInfo.CompareInfo;
    }
}
