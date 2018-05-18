using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MultiCube
{
    class Program
    {
        static ConsoleKeyInfo keyPress;
        static bool altDown, shiftDown;
        const int VSCREEN_HEIGHT = 50, VSCREEN_WIDTH = 50;
        static List<VScreen> Init()
        {
            List<VScreen> screens = new List<VScreen>();
            Console.CursorVisible = false;
            Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;
            Console.Title = "MultiCube (DirectCube 9)";
            SetFullscreen();

            int x = 0, y = 0;
            for (; y < Console.WindowHeight - VSCREEN_HEIGHT + 1; y += VSCREEN_HEIGHT + 1)
            {
                for (; x < Console.WindowWidth - VSCREEN_WIDTH + 1; x += VSCREEN_WIDTH + 1)
                {
                    screens.Add(new VScreen(VSCREEN_HEIGHT, VSCREEN_WIDTH, x, y));
                }
                x = 0;
            }
        }

        static void SetFullscreen()
        {
            /* Note: This method limits the program to 32-bit compatibility, because
               executing this in 64-bit will do nothing as SetConsoleDisplayMode is not supported anymore. */
            IntPtr consoleSession = DllImports.GetStdHandle(DllImports.CONSOLE);   // get handle for current console session
            DllImports.SetConsoleDisplayMode(consoleSession, 1, out _); // set the console to fullscreen
            // Note: 'out _' instantly disposes the out parameter.
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }
        static void Main()
        {
            Init();
            int fheight = Console.WindowHeight;
            int fwidth = Console.WindowWidth;

            VScreen screen = new VScreen(35, 35);
            Cube cube = new Cube();

            // Starting angle
            float angX = 0f, angY = 0f, angZ = 0f;
            // If escape is pressed later, the program will exit
            Random random = new Random();
            bool exit = false;
            bool manualControl = true;
            float rotationFactor;
            while (!exit)
            {
                if (fheight == Console.WindowHeight || fwidth == Console.WindowWidth) ; else SetFullscreen();
                //Console.SetCursorPosition(0, 0);                                                      // Debug
                //Console.Write("Height: " + Console.WindowHeight + "\tWidth: " + Console.WindowWidth); // Debug
                screen.Clear();
                cube.Print2DProjection(angX, angY, angZ, ref screen);
                screen.Refresh();
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
                        case ConsoleKey.D0:
                        case ConsoleKey.NumPad0:
                            break;
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            break;
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            break;
                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            break;
                        case ConsoleKey.D4:
                        case ConsoleKey.NumPad4:
                            break;
                        case ConsoleKey.D5:
                        case ConsoleKey.NumPad5:
                            break;
                        case ConsoleKey.D6:
                        case ConsoleKey.NumPad6:
                            break;
                        case ConsoleKey.D7:
                        case ConsoleKey.NumPad7:
                            break;
                        case ConsoleKey.D8:
                        case ConsoleKey.NumPad8:
                            break;
                        case ConsoleKey.D9:
                        case ConsoleKey.NumPad9:
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
                    angX += random.Next(0, 3);
                    angY += random.Next(0, 3);
                    angZ += random.Next(0, 3);
                    Thread.Sleep(10);
                }
                Console.CursorVisible = false; // Workaround for cursor staying visible if you click into the window once
            }
        }
    }
}
