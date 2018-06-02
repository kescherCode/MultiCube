using System;

namespace MultiCube
{
    // Extension class containing functionality that VScreen wouldn't need for other programs
    static class VScreenExtensions
    {
        static public void PrintBorders(this VScreen screen, ConsoleColor color)
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            //Console.SetCursorPosition(x + XOffset, y + YOffset);

            // Print vertical right-hand screen border
            Console.CursorLeft = screen.XOffset + screen.WindowWidth;
            for (int y = 0; y < screen.WindowHeight; y++)
            {
                Console.CursorTop = screen.YOffset + y;
                Console.Write(VScreen.V_BORDER_CHAR);
                Console.CursorLeft--;
            }

            // Print horizontal bottom screen border
            Console.CursorTop = screen.YOffset + screen.WindowHeight;
            for (int x = 0; x <= screen.WindowWidth; x++)
            {
                Console.CursorLeft = screen.XOffset + x;
                Console.Write(VScreen.H_BORDER_CHAR);
                Console.CursorLeft--;
            }

            Console.ForegroundColor = prevColor;
        }
    }
}
