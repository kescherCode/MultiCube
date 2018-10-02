using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCube
{
    class VScreen
    {
        public bool Changed { get; private set; } = false;
        public bool Idle { get; private set; } = false;
        /* Lines[y, x] because my thought process says "first look at the height (y), then the position on that line (x)" on a grid,
           so that design choice was made because it was easier for me to keep in mind while coding. */
        public char[,] Lines { get; private set; }
        public char[,] PrevLines { get; private set; }

        public int WindowWidth { get; }
        public int WindowHeight { get; }
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }

        public VScreen(int height, int width, int xOffset, int yOffset)
        {
            // We have to initialise both output memories with spaces. empty[,] is serving
            Lines = new char[height, width];
            PrevLines = new char[height, width];
            Parallel.For(0, WindowHeight, y =>
            {
                Parallel.For(0, WindowWidth, x =>
                {
                    Lines[y, x] = PrevLines[y, x] = ' ';
                });
            });

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
            Idle = true;
        }
        /// <summary>
        /// Moves the screen offset. Optionally clears screen at old position and/or outputs at new position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveOffset(int x = 0, int y = 0, bool clearBeforehand = false, bool outputAfterMove = false)
        {
            if (Idle)
            {
                Idle = false;
                char[,] lines = new char[WindowHeight, WindowWidth];
                Parallel.For(0, WindowHeight, y_ =>
                {
                    Parallel.For(0, WindowWidth, x_ =>
                    {
                        lines[y_, x_] = Lines[y_, x_];
                    });
                });
                if (clearBeforehand) { Clear(); Refresh(); }
                XOffset += x;
                YOffset += y;
                if (outputAfterMove)
                {
                    Parallel.Invoke(
                        () => Changed = true,
                        () =>
                        Parallel.For(0, WindowHeight, y_ =>
                        {
                            Parallel.For(0, WindowWidth, x_ =>
                            {
                                PrevLines[y_, x_] = ' ';
                            });
                        }),
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
        public void Push(string text, int x, int y)
        {
            if (Idle)
            {
                Idle = false;
                Changed = true;
                if (text.Length <= WindowWidth - x - 1 && y <= WindowHeight - 1)
                {
                    Parallel.For(x, text.Length, i =>
                    {
                        Lines[y, x] = text[i];
                    });
                }
                else throw new ArgumentException($"String was too long for that position or you picked the wrong x/y coordinates.\ns: {text} x: {x} y: {y}\nMaximum value of x: {WindowWidth - 1}\nMaximum value of y: {WindowHeight - 1}");
            }
            else
            {
                SpinWait.SpinUntil(() => Idle);
                Push(text, x, y);
            }
            Idle = true;
        }
        public void Push(char symbol, int x, int y)
        {
            if (Idle)
            {
                Idle = false;
                Changed = true;
                if (x <= WindowWidth - 1 && y <= WindowHeight - 1)
                {
                    Lines[y, x] = symbol;
                }
                else throw new ArgumentException($"You picked the wrong x/y coordinates.\nc: {symbol} x: {x} y: {y}\nMaximum value of x: {WindowWidth - 1}\nMaximum value of y: {WindowHeight - 1}");
            }
            else
            {
                SpinWait.SpinUntil(() => Idle);
                Push(symbol, x, y);
            }
            Idle = true;
        }
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
                            for (int y = 0; y < WindowHeight; y++)
                                for (int x = 0; x < WindowWidth; x++)
                                    if (Lines[y, x] != PrevLines[y, x])
                                    {
                                        Console.SetCursorPosition(x + XOffset, y + YOffset);
                                        Parallel.Invoke(
                                        () => Console.Write(Lines[y, x]),
                                        () => PrevLines[y, x] = Lines[y, x]);
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
        public void FullOutput()
        {
            if (Idle)
            {
                Idle = false;
                for (int y = 0; y < WindowHeight; y++)
                    for (int x = 0; x < WindowWidth; x++)
                    {
                        Console.SetCursorPosition(x + XOffset, y + YOffset);
                        Parallel.Invoke(
                            () => Console.Write(Lines[y, x]),
                            () => PrevLines[y, x] = Lines[y, x]
                        );
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
        public void Clear()
        {
            if (Idle)
            {
                Idle = false;
                Parallel.Invoke(
                () => Changed = true,
                () =>
                {
                    Parallel.For(0, WindowHeight, y =>
                    {
                        Parallel.For(0, WindowWidth, x =>
                        {
                            Lines[y, x] = ' ';
                        });
                    });
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
