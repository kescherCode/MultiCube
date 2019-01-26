using System;

namespace MultiCube
{
    /// <summary>
    ///     Stores "global" variables.
    /// </summary>
    internal struct Globals
    {
        // Object used for locking and synchronizing access to console.
        public static readonly object ConsoleLock = new object();

        // The maximum amount of screens that should exist.
        public const int ScreenCount = 10;

        // Char used for a cube's foreground.
        public const char CubeCharFG = 'o';

        // Char used for a cube's background.
        public const char CubeCharBG = '-';

        // Different rotation factors
        public const double NormalFactor = 6d, DoubleFactor = NormalFactor * 2d, HalfFactor = NormalFactor / 2d;

        // We only need one PRNG.
        public static readonly Random Random = new Random(10000);
    }
}