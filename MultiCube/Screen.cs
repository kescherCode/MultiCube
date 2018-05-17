using System;

namespace MultiCube
{
    class Screen
    {
        /* Lines[y, x] because my thought process says "first look at the height (y), then the position on that line (x)" on a grid,
           so that design choice was made because it was easier for me to keep in mind while coding. */

        public char[,] Lines { get; private set; } // Not List<string> because a String's indexer is read-only.
        public char[,] PrevLines { get; private set; }
        public int LineWidth { get; }
        public int LineCount { get; }
        public int XOffset { get; }
        public int YOffset { get; }

        public Screen(int height, int width, int xOffset = 0, int yOffset = 0)
        {
            // We have to initialise both output memories with spaces
            Lines = new char[height, width];
            for (int y = 0; y < LineCount; y++)
                for (int x = 0; x < LineWidth; x++)
                    Lines[y, x] = PrevLines[y, x] = ' ';

            LineWidth = width;
            LineCount = height;
            this.XOffset = xOffset;
            this.YOffset = yOffset;
        }
        public void Push(string s, int x, int y)
        {
            if (s.Length <= LineWidth - x - 1 && y <= LineCount)
            {
                for (int i = x; i < s.Length; i++, x++)
                {
                    Lines[y, x] = s[i];
                }
            }
            else throw new ArgumentException($"String was too long for that position or you picked the wrong x/y coordinates.\ns: {s} x: {x} y: {y}\nMaximum value of x: {LineWidth - 1}\nMaximum value of y: {LineCount - 1}");
        }
        public void Push(char c, int x, int y)
        {
            if (x <= LineWidth - 1 && y <= LineCount - 1)
            {
                Lines[y, x] = c;
            }
            else throw new ArgumentException($"You picked the wrong x/y coordinates.\nc: {c} x: {x} y: {y}\nMaximum value of x: {LineWidth - 1}\nMaximum value of y: {LineCount - 1}");
        }
        public void Refresh()
        {
            for (int y = 0; y < LineCount; y++)
                for (int x = 0; x < LineWidth; x++)
                    if (Lines[y, x] != PrevLines[y, x])
                    {
                        Console.SetCursorPosition(x + XOffset, y + YOffset);
                        Console.Write(Lines[y, x]);
                        PrevLines[y, x] = Lines[y, x];
                    }
        }
        public void Clear()
        {
            for (int y = 0; y < LineCount; y++)
                for (int x = 0; x < LineWidth; x++)
                {
                    Console.SetCursorPosition(x + XOffset, y + YOffset);
                    Console.Write(' ');
                }
        }
    }
}
