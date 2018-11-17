using System;
using System.Threading;
using System.Threading.Tasks;
using static MultiCube.Globals;

namespace MultiCube
{
    /// <summary>
    /// A class used for creating virtually seperated screens in a console.
    /// </summary>
    internal class VScreen
    {
        // Determines if there are any changes to the output at all (in order to save execution time in Refresh())
        public bool Changed { get; private set; } = false;

        // Determines if a method has to wait for another method in an instance of VScreen to complete in order to avoid issues with race conditions
        public bool Idle { get; private set; } = false;

        // The two screen buffers, inspired by how graphics cards handle display output.
        public char[,] Lines { get; private set; }
        public char[,] PrevLines { get; private set; }

        public int WindowWidth { get; }

        public int WindowHeight { get; }

        // Offset the screen should have on the console output.
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }

        /// <summary>
        /// Accesses a char inside the screen.
        /// </summary>
        /// <param name="x">x-coordinate of the char</param>
        /// <param name="y">y-coordinate of the char</param>
        /// <returns>The char assigned to the coordinate</returns>
        public char this[int x, int y]
        {
            get => Lines[x, y];
            set => Push(value, x, y);
        }

        /// <summary>
        /// Initializes a new VScreen instance.
        /// </summary>
        /// <param name="width">Width of the virtual screen</param>
        /// <param name="height">Height of the virtual screen</param>
        /// <param name="xOffset">Horizontal offset of the top-left corner of the VScreen.</param>
        /// <param name="yOffset">Vertical offset of the top-left corner of the VScreen.</param>
        public VScreen(int width, int height, int xOffset, int yOffset)
        {
            // We have to initialise both output memories with spaces. empty[,] is serving
            Lines = new char[width, height];
            PrevLines = new char[width, height];
            Parallel.For(0, width, x => { Parallel.For(0, height, y => { Lines[x, y] = PrevLines[x, y] = ' '; }); });

            if (width > Console.WindowWidth)
                throw new ArgumentOutOfRangeException(nameof(width), "width was greater than the console window's width.");
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width), "width requires a positive value.");

            if (height > Console.WindowHeight)
                throw new ArgumentOutOfRangeException(nameof(height), "height was greater than the console window height.");
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height), "height requires a positive value.");

            if (width + xOffset > Console.WindowWidth)
                throw new ArgumentOutOfRangeException(nameof(xOffset), "xOffset + width was greater than the console window width.");

            if (height + yOffset > Console.WindowHeight)
                throw new ArgumentOutOfRangeException(nameof(yOffset), "xOffset + width was greater than the console window width.");

            WindowWidth = width;
            WindowHeight = height;
            XOffset = xOffset;
            YOffset = yOffset;
            Idle = true;
        }

        /// <summary>
        /// Moves the screen offset. Optionally clears screen at old position and/or outputs at new position.
        /// </summary>
        /// <param name="x">Defines by how many characters the screen is moved horizontally</param>
        /// <param name="y">Defines by how many characters the screen is moved vertically</param>
        /// <param name="clearBeforehand">Defines if the screen should clean up the area it used before moving.</param>
        /// <param name="outputAfterMove">Defines if the screen should output its contents after being moved.</param>
        public void MoveOffset(int x = 0, int y = 0, bool clearBeforehand = false, bool outputAfterMove = false)
        {
            if (Idle)
            {
                Idle = false;
                var lines = new char[WindowWidth, WindowHeight];
                Parallel.For(0, WindowWidth,
                    xi => { Parallel.For(0, WindowHeight, y_ => { lines[xi, y_] = Lines[xi, y_]; }); });
                if (clearBeforehand)
                {
                    Clear();
                    Refresh();
                }

                XOffset += x;
                YOffset += y;
                if (outputAfterMove)
                {
                    Parallel.Invoke(
                        () => Changed = true,
                        () =>
                            Parallel.For(0, WindowWidth,
                                xi => { Parallel.For(0, WindowHeight, y_ => { PrevLines[xi, y_] = ' '; }); }),
                        () => Lines = lines
                    );
                    Refresh();
                }
            }
            else
            {
                SpinWait.SpinUntil(() => Idle);
                MoveOffset(x, y, clearBeforehand, outputAfterMove);
            }

            Idle = true;
        }

        /// <summary>
        /// Pushes a char to the screen buffer.
        /// </summary>
        /// <param name="symbol">char to be pushed to the screen</param>
        /// <param name="x">x-Coordinate of the char</param>
        /// <param name="y">y-Coordinate of the char</param>
        public void Push(char symbol, int x, int y)
        {
            if (Idle)
            {
                Idle = false;
                Changed = true;
                if (x <= WindowWidth - 1 && y <= WindowHeight - 1)
                {
                    Lines[x, y] = symbol;
                }
                else
                    throw new ArgumentException(
                        $"You picked the wrong x/y coordinates.\nsymbol: {symbol} x: {x} y: {y}\nMaximum value of x: {WindowWidth - 1}\nMaximum value of y: {WindowHeight - 1}");
            }
            else
            {
                SpinWait.SpinUntil(() => Idle);
                Push(symbol, x, y);
            }

            Idle = true;
        }

        /// <summary>
        /// Outputs all characters that have changed on the buffer since the last output.
        /// Using this method in an unsynchronized way will cause it to slow down.
        /// </summary>
        public void Refresh()
        {
            if (Idle)
            {
                Idle = false;
                if (Changed)
                {
                    Parallel.Invoke(
                        () =>
                        {
                            for (int x = 0; x < WindowWidth; x++)
                                for (int y = 0; y < WindowHeight; y++)
                                    if (Lines[x, y] != PrevLines[x, y])
                                    {
                                        Parallel.Invoke(() =>
                                            {
                                                lock (ConsoleLock)
                                                {
                                                    Console.SetCursorPosition(x + XOffset, y + YOffset);
                                                    Console.Write(Lines[x, y]);
                                                }
                                            },
                                            () => PrevLines[x, y] = Lines[x, y]);
                                    }
                        },
                        () => Changed = false
                    );
                }
            }
            else
            {
                SpinWait.SpinUntil(() => Idle);
                Refresh();
            }

            Idle = true;
        }

        /// <summary>
        /// Outputs every single character on the screen buffer.
        /// If you need fast output, use Refresh() instead.
        /// </summary>
        public void FullOutput()
        {
            if (Idle)
            {
                Idle = false;
                for (int x = 0; x < WindowWidth; x++)
                    for (int y = 0; y < WindowHeight; y++)
                    {
                        Parallel.Invoke(() =>
                            {
                                lock (ConsoleLock)
                                {
                                    Console.SetCursorPosition(x + XOffset, y + YOffset);
                                    Console.Write(Lines[x, y]);
                                }
                            },
                            () => PrevLines[x, y] = Lines[x, y]);
                    }

                Changed = false;
            }
            else
            {
                SpinWait.SpinUntil(() => Idle);
                FullOutput();
            }

            Idle = true;
        }

        /// <summary>
        /// Clears the output buffer.
        /// </summary>
        public void Clear()
        {
            if (Idle)
            {
                Idle = false;
                Parallel.Invoke(
                    () => Changed = true,
                    () =>
                    {
                        Parallel.For(0, WindowWidth,
                            x => { Parallel.For(0, WindowHeight, y => { Lines[x, y] = ' '; }); });
                    });
            }
            else
            {
                SpinWait.SpinUntil(() => Idle);
                Clear();
            }

            Idle = true;
        }
    }
}