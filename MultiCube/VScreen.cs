using System;
using System.Text;
using static MultiCube.Globals;

namespace MultiCube
{
    /// <summary>
    ///     A class used for creating virtually seperated screens in a console.
    /// </summary>
    internal class VScreen
    {
        private readonly char[,] _empty;

        /// <summary>
        ///     Initializes a new VScreen instance.
        /// </summary>
        /// <param name="width">Width of the virtual screen</param>
        /// <param name="height">Height of the virtual screen</param>
        /// <param name="xOffset">Horizontal offset of the top-left corner of the VScreen.</param>
        /// <param name="yOffset">Vertical offset of the top-left corner of the VScreen.</param>
        public VScreen(int width, int height, int xOffset, int yOffset)
        {
            if (width > Console.WindowWidth)
                throw new ArgumentOutOfRangeException(nameof(width),
                    "width was greater than the console window's width.");
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width), "width requires a positive value.");

            if (height > Console.WindowHeight)
                throw new ArgumentOutOfRangeException(nameof(height),
                    "height was greater than the console window height.");
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height), "height requires a positive value.");

            if (width + xOffset > Console.WindowWidth)
                throw new ArgumentOutOfRangeException(nameof(xOffset),
                    "xOffset + width was greater than the console window width.");

            if (height + yOffset > Console.WindowHeight)
                throw new ArgumentOutOfRangeException(nameof(yOffset),
                    "yOffset + width was greater than the console window width.");

            Grid = new char[width, height];
            _empty = new char[width, height];
            for (var x = 0; x < width; ++x)
            for (var y = 0; y < height; ++y)
                Grid[x, y] = _empty[x, y] = ' ';

            WindowWidth = width;
            WindowHeight = height;
            XOffset = xOffset;
            YOffset = yOffset;
        }

        // Determines if there are any changes to the output at all (in order to save execution time in Output())
        private bool Changed { get; set; }

        // Content of the screen
        private char[,] Grid { get; set; }

        public int WindowWidth { get; }
        public int WindowHeight { get; }

        // Offset the screen should have on the console output.
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }

        /// <summary>
        ///     Accesses a char inside the screen.
        /// </summary>
        /// <param name="x">x-coordinate of the char</param>
        /// <param name="y">y-coordinate of the char</param>
        /// <returns>The char assigned to the coordinate</returns>
        // ReSharper disable once UnusedMember.Global
        public char this[int x, int y]
        {
            get => Grid[x, y];
            // ReSharper disable once UnusedMember.Global
            set => Push(value, x, y);
        }

        /// <summary>
        ///     Moves the screen offset by a certain amount. Optionally clears screen at old position and/or outputs at new
        ///     position.
        /// </summary>
        /// <param name="xMove">Defines by how many characters the screen is moved horizontally</param>
        /// <param name="yMove">Defines by how many characters the screen is moved vertically</param>
        /// <param name="clearBeforehand">Defines if the screen should clean up the area it used before moving.</param>
        /// <param name="outputAfterMove">Defines if the screen should output its contents after being moved.</param>
        // ReSharper disable once UnusedMember.Global
        public void MoveOffset(int xMove = 0, int yMove = 0, bool clearBeforehand = false, bool outputAfterMove = false)
        {
            var lines = new char[WindowWidth, WindowHeight];

            for (var x = 0; x < WindowWidth; ++x)
            for (var y = 0; y < WindowHeight; ++y)
                lines[x, y] = Grid[x, y];

            if (clearBeforehand)
            {
                Clear();
                Output();
            }

            XOffset += xMove;
            YOffset += yMove;

            if (!outputAfterMove) return;
            Changed = true;
            Grid = lines;
            Output();
        }

        /// <summary>
        ///     Pushes a char to the screen buffer.
        /// </summary>
        /// <param name="symbol">char to be pushed to the screen</param>
        /// <param name="x">x-Coordinate of the char</param>
        /// <param name="y">y-Coordinate of the char</param>
        public void Push(char symbol, int x, int y)
        {
            if (x <= WindowWidth - 1 && y <= WindowHeight - 1)
            {
                if (Grid[x, y] == symbol) return;
                Changed = true;
                Grid[x, y] = symbol;
            }
            else
            {
                throw new ArgumentException(
                    $"You picked the wrong x/y coordinates.\nc: {symbol} x: {x} y: {y}\nMaximum value of x: {WindowWidth - 1}\nMaximum value of y: {WindowHeight - 1}");
            }
        }

        /// <summary>
        ///     Outputs everything on the buffer if anything has changed.
        /// </summary>
        public void Output()
        {
            if (!Changed) return;

            var line = new StringBuilder(WindowWidth);
            for (var y = 0; y < WindowHeight; ++y)
            {
                line.Clear();
                for (var x = 0; x < WindowWidth; ++x) line.Append(Grid[x, y]);

                lock (ConsoleLock)
                {
                    Console.SetCursorPosition(XOffset, y + YOffset);
                    Console.Write(line);
                }
            }

            Changed = false;
        }

        /// <summary>
        ///     Clears the output buffer.
        /// </summary>
        public void Clear()
        {
            Grid = (char[,]) _empty.Clone();
            Changed = true;
        }
    }
}