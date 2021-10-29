/*           INFINITY CODE          */
/*     https://infinity-code.com    */

namespace InfinityCode.uContext.Tools
{
    public static partial class WindowHistory
    {
        public abstract class Provider
        {
            public abstract float order { get; }

            public abstract void GenerateMenu(GenericMenuEx menu, ref bool hasItems);
        }
    }
}