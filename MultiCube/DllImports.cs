using System;
using System.Runtime.InteropServices;

namespace MultiCube
{
    static class DllImports
    {
        /* Source: https://social.msdn.microsoft.com/Forums/vstudio/en-US/0b374f73-5604-48ee-a720-53bb5b19467b/maximizing-the-console-window
          Modified to only include what I need to make the console window go fullscreen*/
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
