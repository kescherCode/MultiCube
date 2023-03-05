﻿using System;
using System.Text;
using static MultiCube.Globals;

namespace MultiCube
{
    /// <summary>
    ///     Extension methods for VScreen.
    /// </summary>
    internal static class VScreenExtensions
    {
        /// <summary>
        ///     Prints borders around the screen.
        /// </summary>
        /// <param name="screen">VScreen to have its borders printed</param>
        /// <param name="verticalBorderChar">Border for the right-hand border.</param>
        /// <param name="horizontalBorderChar">Border for the bottom border</param>
        /// <param name="color">Optional ConsoleColor to print the borders in</param>
        public static void PrintBorders(this VScreen screen, char horizontalBorderChar = '-',
            char verticalBorderChar = '|', ConsoleColor color = ConsoleColor.White)
        {
            lock (ConsoleLock)
            {
                var prevColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                // Print vertical right-hand screen border
                Console.CursorLeft = screen.XOffset + screen.WindowWidth;
                for (var y = 0; y < screen.WindowHeight; ++y)
                    try
                    {
                        Console.CursorTop = screen.YOffset + y;
                        Console.Write(verticalBorderChar);
                        // Move the cursor left after writing
                        --Console.CursorLeft;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        /* ignore, user error with offset or VScreen size */
                    }

                // Print horizontal bottom screen border
                Console.CursorTop = screen.YOffset + screen.WindowHeight;
                Console.CursorLeft = screen.XOffset;
                var sb = new StringBuilder();
                for (var x = 0; x <= screen.WindowWidth; ++x)
                    sb.Append(horizontalBorderChar);
                Console.Write(sb);

                Console.ForegroundColor = prevColor;
            }
        }
    }
}