using System;
using static MultiCube.Globals;

namespace MultiCube
{
    /// <summary>
    /// Extension methods for VScreen.
    /// </summary>
    internal static class VScreenBorders
    {
        /// <summary>
        /// Prints borders around the screen. Requires the screen to be one char away in all directions from other screens in order to avoid
        /// </summary>
        /// <param name="screen">VScreen to have its borders printed</param>
        /// <param name="color">Optional ConsoleColor to print the borders in</param>
        public static void PrintBorders(this VScreen screen, ConsoleColor color = ConsoleColor.White)
        {
            lock (ConsoleLock)
            {
                ConsoleColor prevColor = Console.ForegroundColor;
                Console.ForegroundColor = color;

                // Print vertical right-hand screen border
                Console.CursorLeft = screen.XOffset + screen.WindowWidth;
                for (int y = 0; y < screen.WindowHeight; y++)
                {
                    try
                    {
                        Console.CursorTop = screen.YOffset + y;
                        Console.Write(VBorderChar);
                        // Move the cursor left after writing
                        Console.CursorLeft--;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        /* ignore, "user error" with offset or VScreen size */
                    }
                }

                // Print horizontal bottom screen border
                Console.CursorTop = screen.YOffset + screen.WindowHeight;
                for (int x = 0; x <= screen.WindowWidth; x++)
                {
                    try
                    {
                        Console.CursorLeft = screen.XOffset + x;
                        Console.Write(HBorderChar);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        /* ignore, "user error" with offset or VScreen size */
                    }
                }

                Console.ForegroundColor = prevColor;
            }
        }
    }
}