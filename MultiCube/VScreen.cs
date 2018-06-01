using System;

namespace MultiCube
{
    class VScreen
    {
        private readonly char[,] empty;
        public const char H_BORDER_CHAR = '-';
        public const char V_BORDER_CHAR = '|';

        public bool Changed { get; private set; } = false;
        /* Lines[y, x] because my thought process says "first look at the height (y), then the position on that line (x)" on a grid,
           so that design choice was made because it was easier for me to keep in mind while coding. */
        public char[,] Lines { get; private set; } // Not List<string> because a String's indexer is read-only.
        public char[,] PrevLines { get; private set; }
        public int WindowWidth { get; }
        public int WindowHeight { get; }
        public int XOffset { get; }
        public int YOffset { get; }

        public VScreen(int height, int width, int xOffset, int yOffset)
        {
            // We have to initialise both output memories with spaces. empty[,] is serving
            Lines = new char[height, width];
            PrevLines = new char[height, width];
            empty = new char[height, width];
            for (int y = 0; y < WindowHeight; y++)
                for (int x = 0; x < WindowWidth; x++)
                    Lines[y, x] = PrevLines[y, x] = empty[y, x] = ' ';

            if (width > Console.WindowWidth) throw new ArgumentOutOfRangeException("width was greater than the console window's width.");
            if (width < 1) throw new ArgumentOutOfRangeException("width requires a positive value.");

            if (height > Console.WindowHeight) throw new ArgumentOutOfRangeException("height was greater than the console window height.");
            if (height < 1) throw new ArgumentOutOfRangeException("height requires a positive value.");

            if (width + xOffset > Console.WindowWidth) throw new ArgumentOutOfRangeException("xOffset + width was greater than the console window width.");

            if (height + yOffset > Console.WindowHeight) throw new ArgumentOutOfRangeException("xOffset + width was greater than the console window width.");

            WindowWidth = width;
            WindowHeight = height;
            XOffset = xOffset;
            YOffset = yOffset;
        }
        public void Push(string s, int x, int y)
        {
            Changed = true;
            if (s.Length <= WindowWidth - x - 1 && y <= WindowHeight - 1)
            {
                for (int i = x; i < s.Length; i++, x++)
                {
                    Lines[y, x] = s[i];
                }
            }
            else throw new ArgumentException($"String was too long for that position or you picked the wrong x/y coordinates.\ns: {s} x: {x} y: {y}\nMaximum value of x: {WindowWidth - 1}\nMaximum value of y: {WindowHeight - 1}");
        }
        public void Push(char c, int x, int y)
        {
            Changed = true;
            if (x <= WindowWidth - 1 && y <= WindowHeight - 1)
            {
                Lines[y, x] = c;
            }
            else throw new ArgumentException($"You picked the wrong x/y coordinates.\nc: {c} x: {x} y: {y}\nMaximum value of x: {WindowWidth - 1}\nMaximum value of y: {WindowHeight - 1}");

        }
        public void Refresh()
        {
            if (Changed)
            {
                for (int y = 0; y < WindowHeight; y++)
                    for (int x = 0; x < WindowWidth; x++)
                        if (Lines[y, x] != PrevLines[y, x])
                        {
                            Console.SetCursorPosition(x + XOffset, y + YOffset);
                            Console.Write(Lines[y, x]);
                            PrevLines[y, x] = Lines[y, x];
                        }
                Changed = false;
            }
        }
        public void FullOutput()
        {
            for (int y = 0; y < WindowHeight; y++)
                for (int x = 0; x < WindowWidth; x++)
                {
                    Console.SetCursorPosition(x + XOffset, y + YOffset);
                    Console.Write(Lines[y, x]);
                }
            Changed = false;
        }
        public void Clear()
        {
            Changed = true;
            Lines = (char[,])empty.Clone();
        }
    }
}
