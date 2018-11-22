using System;

namespace MultiCube
{
    static class Globals
    {
        // Object used for locking and synchronizing access to console.
        public static readonly object consoleLock = new object();

        public const int SCREEN_COUNT = 10;

        public const double ZOOM_FACTOR = 3.2f;

        // Screen border chars.
        public const char H_BORDER_CHAR = '-';
        public const char V_BORDER_CHAR = '|';

        // Char used for a cube's lines.
        public const char CUBE_CHAR = 'o';

        public const double SPEED = 6d, DOUBLE_SPEED = 12d, HALF_SPEED = 3d; // User control speeds

        // We only need one PRNG.
        public static Random random = new Random();
    }
}
