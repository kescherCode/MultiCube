using System;
using System.Threading;

namespace MultiCube
{
    static class VScreenBorders
    {
        // It's okay for it to have its own idle variable, doesn't interfere with anything
        static bool idle = true;
        static public void PrintBorders(this VScreen screen, ConsoleColor color = ConsoleColor.White)
        {
            if (idle)
            {
                idle = false;
                ConsoleColor prevColor = Console.ForegroundColor;
                Console.ForegroundColor = color;

                // Print vertical right-hand screen border
                Console.CursorLeft = screen.XOffset + screen.WindowWidth;
                for (int y = 0; y < screen.WindowHeight; y++)
                {
                    Console.CursorTop = screen.YOffset + y;
                    Console.Write(Globals.V_BORDER_CHAR);
                    // Move the cursor back after a write
                    Console.CursorLeft--;
                }

                // Print horizontal bottom screen border
                Console.CursorTop = screen.YOffset + screen.WindowHeight;
                for (int x = 0; x <= screen.WindowWidth; x++)
                {
                    Console.CursorLeft = screen.XOffset + x;
                    Console.Write(Globals.H_BORDER_CHAR);
                }

                Console.ForegroundColor = prevColor;
            }
            else
            {
                SpinWait.SpinUntil(() => idle);
                PrintBorders(screen, color);
            }
            idle = true;
        }
    }
}
