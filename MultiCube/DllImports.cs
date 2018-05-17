using System;
using System.Runtime.InteropServices;

namespace MultiCube
{
    static class DllImports
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Coords
        {
            public short X;
            public short Y;
            public Coords(short x, short y)
            {
                X = x;
                Y = y;
            }

        }
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int handle);
        public const int CONSOLE = -11;
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleDisplayMode(IntPtr ConsoleOutput, uint Flags, out Coords NewScreenBufferDimensions);
    }
}
