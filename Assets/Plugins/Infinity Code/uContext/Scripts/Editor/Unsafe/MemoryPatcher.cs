/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace InfinityCode.uContext.Unsafe
{
    public unsafe class MemoryPatcher
    {
        private IntPtr body;
        private IntPtr borrowed;
        private byte[] backup;
        private bool patched;

        public bool IsPatched()
        {
            return patched;
        }

        public void Restore()
        {
            if (!patched) return;
            byte* c = (byte*)body.ToPointer();
            for (int i = 0; i < backup.Length; i++) *(c++) = backup[i];
        }

        public void Swap(MethodInfo original, MethodInfo replacement)
        {
            if (original == null || replacement == null) return;
            RuntimeHelpers.PrepareMethod(original.MethodHandle);
            RuntimeHelpers.PrepareMethod(replacement.MethodHandle);

            body = original.MethodHandle.GetFunctionPointer();
            borrowed = replacement.MethodHandle.GetFunctionPointer();

            byte* p1 = (byte*)body.ToPointer();
            byte* p2 = (byte*)borrowed.ToPointer();

            backup = new byte[25];

            for (int i = 0; i < backup.Length; i++) backup[i] = *(p1 + i);

            if (sizeof(IntPtr) == 8)
            {
                byte* c = p1;
                *(c++) = 0x68;
                *((uint*)c) = (uint)p2;
                c += 4;
                *(c++) = 0xC7;
                *(c++) = 0x44;
                *(c++) = 0x24;
                *(c++) = 0x04;
                *((uint*)c) = (uint)((ulong)p2 >> 32);
                c += 4;
                *(c++) = 0xC3;
            }
            else
            {
                *p1 = 0x68;
                *((uint*)(p1 + 1)) = (uint)p2;
                *(p1 + 5) = 0xC3;
            }

            patched = true;
        }

        public static MemoryPatcher SwapMethods(MethodInfo original, MethodInfo replacement)
        {
            MemoryPatcher patcher = new MemoryPatcher();
            patcher.Swap(original, replacement);
            return patcher;
        }
    }
}