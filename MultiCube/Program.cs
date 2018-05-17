using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MultiCube
{
    class Screen
    {
        // lines[y, x] because my thought process says "first look at the height, then the position on that line" on a grid.
        private char[,] lines; // Not List<string> because a String's indexer is read-only.
        private char[,] prevLines;
        int lineWidth, lineCount, xOffset, yOffset;

        public Screen(int height, int width, int xOffset = 0, int yOffset = 0)
        {
            Console.Clear();
            lines = new char[height, width];
            for (int y = 0; y < lineCount; y++)
                for (int x = 0; x < lineWidth; x++)
                    lines[y, x] = ' ';
            prevLines = (char[,])lines.Clone();

            lineWidth = width;
            lineCount = height;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }
        public void Push(string s, int x, int y)
        {
            if (s.Length <= lineWidth - x - 1 && y <= lineCount)
            {
                for (int i = x; i < s.Length; i++, x++)
                {
                    lines[y, x] = s[i];
                }
            }
            else throw new ArgumentException($"String was too long for that position or you picked the wrong x/y coordinates.\ns: {s} x: {x} y: {y}\nMaximum value of x: {lineWidth - 1}\nMaximum value of y: {lineCount - 1}");
        }
        public void Push(char c, int x, int y)
        {
            if (x <= lineWidth - 1 && y <= lineCount - 1)
            {
                lines[y, x] = c;
            }
            else throw new ArgumentException($"You picked the wrong x/y coordinates.\nc: {c} x: {x} y: {y}\nMaximum value of x: {lineWidth - 1}\nMaximum value of y: {lineCount - 1}");
        }
        public void Refresh()
        {
            for (int y = 0; y < lineCount; y++)
                for (int x = 0; x < lineWidth; x++)
                    if (lines[y, x] != prevLines[y, x])
                    {
                        Console.SetCursorPosition(x + xOffset, y + yOffset);
                        Console.Write(lines[y, x]);
                        prevLines[y, x] = lines[y, x];
                    }
        }
    }
    class Program
    {
        const int LEDGE_LENGTH = 25;
        static IEnumerable<CornerData> lines;
        static ConsoleKeyInfo keyPress;
        static bool altDown, shiftDown;
        class CornerData
        {
            public Point3D a;
            public Point3D b;
            public CornerData(Point3D a, Point3D b)
            {
                this.a = a;
                this.b = b;
            }
        }
        static void Init()
        {
            Console.CursorVisible = false;
            Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;
            Console.Title = "Press ESC to exit | Rotating Cube Demo by Jeremy Kescher";
            SetFullscreen();
        }
        static void Print2DProjection(float angX, float angY, float angZ)
        {
            foreach (CornerData line in lines)
            {
                for (int i = 0; i < LEDGE_LENGTH; i++)
                {
                    // Find a point between A and B by following formula p=a+z(b-a) where z
                    // is a value between 0 and 1.
                    var point = line.a + (i * 1.0f / 24) * (line.b - line.a);
                    // Rotates the point relative to all the angles given to the method.
                    Point3D r = point.RotateX(angX).RotateY(angY).RotateZ(angZ);
                    // Projects the point into 2d space. Acts as a kind of camera setting.
                    Point3D q = r.Project(0, 0, 200, 3);
                    // Setting the cursor to the proper positions
                    int x = ((int)(q.x + Console.WindowWidth * 2.5) / 5);
                    int y = ((int)(q.y + Console.WindowHeight * 2.5) / 5);
                    Console.SetCursorPosition(x, y);

                    Console.Write('°'); // Max Wichmann suggested this symbol
                }
            }
        }

        static void SetFullscreen()
        {
            IntPtr consoleSession = DllImports.GetStdHandle(DllImports.CONSOLE);   // get handle for current console session
            DllImports.SetConsoleDisplayMode(consoleSession, 1, out _); // set the console to fullscreen
            // Note: 'out _' instantly disposes the out parameter.
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }
        static void Main()
        {
            Init();

            // Simply a list of all possible corners of a cube in 3D space
            List<Point3D> corners = new List<Point3D>
            {
                new Point3D(-1, -1, -1),
                new Point3D(1, -1, -1),
                new Point3D(1, -1, 1),
                new Point3D(-1, -1, 1),
                new Point3D(-1, 1, 1),
                new Point3D(-1, 1, -1),
                new Point3D(1, 1, -1),
                new Point3D(1, 1, 1)
            };

            // A LINQ query getting all corners neccessary for 2D space, returning them to a class-wide struct
            lines = from a in corners
                    from b in corners
                    where (a - b).Length == 2 && a.x + a.y + a.z > b.x + b.y + b.z
                    select new CornerData(a, b);

            // Starting angle
            float angX = 0f, angY = 0f, angZ = 0f;
            // If escape is pressed later, the program will exit
            Random randFactor = new Random();
            bool exit = false;
            bool manualControl = true;
            float rotationFactor;
            while (!exit)
            {
                SetFullscreen();
                //Console.Write("Height: " + Console.WindowHeight + "\tWidth: " + Console.WindowWidth); // Debug
                Print2DProjection(angX, angY, angZ);
                if (manualControl)
                {
                    keyPress = Console.ReadKey(true);
                    altDown = (keyPress.Modifiers & ConsoleModifiers.Alt) != 0;
                    shiftDown = (keyPress.Modifiers & ConsoleModifiers.Shift) != 0;
                    if (shiftDown)
                    {
                        if (altDown) rotationFactor = 1f;
                        else rotationFactor = 0.5f;
                    }
                    else if (altDown) rotationFactor = 2f;
                    else rotationFactor = 1f;
                    switch (keyPress.Key)
                    {
                        case ConsoleKey.W:
                            angX += rotationFactor;
                            break;
                        case ConsoleKey.A:
                            angY += rotationFactor;
                            break;
                        case ConsoleKey.S:
                            angX -= rotationFactor;
                            break;
                        case ConsoleKey.D:
                            angY -= rotationFactor;
                            break;
                        case ConsoleKey.J:
                            angZ += rotationFactor;
                            break;
                        case ConsoleKey.K:
                            angZ -= rotationFactor;
                            break;
                        case ConsoleKey.R:
                            angX = 0f; angY = 0f; angZ = 0f;
                            break;
                        case ConsoleKey.M:
                            manualControl = false;
                            break;
                        case ConsoleKey.Escape:
                            exit = true;
                            break;
                    }
                }
                else
                {
                    if (Console.KeyAvailable)
                    {
                        keyPress = Console.ReadKey(true);
                        switch (keyPress.Key)
                        {
                            case ConsoleKey.M:
                                manualControl = true;
                                break;
                            case ConsoleKey.Escape:
                                exit = true;
                                break;
                        }
                    }
                    angX += randFactor.Next(0, 3);
                    angY += randFactor.Next(0, 3);
                    angZ += randFactor.Next(0, 3);
                    System.Threading.Thread.Sleep(10);
                }
                Console.Clear();
                Console.CursorVisible = false; // Workaround for cursor being visible if you click into the window
            }
        }
    }
}
