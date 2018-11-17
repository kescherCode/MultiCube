using System;

namespace MultiCube
{
    internal static class Globals
    {
        // Object used for locking and synchronizing access to console.
        public static readonly object ConsoleLock = new object();

        public const int ScreenCount = 10;

        public const double ZoomFactor = 3.2f;

        // Screen border chars.
        public const char HBorderChar = '-';
        public const char VBorderChar = '|';

        // Char used for a cube's lines.
        public const char CubeChar = 'o';

        public const double Speed = 6d, DoubleSpeed = 12d, HalfSpeed = 3d; // User control speeds

        // We only need one PRNG.
        public static Random Random = new Random();
    }
}