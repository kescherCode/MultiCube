using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MultiCube
{
    class Program
    {
        const int VSCREEN_HEIGHT = 30, VSCREEN_WIDTH = 50;
        const float ZOOM_FACTOR = 3.2f;
        static void Intro()
        {
            Console.WriteLine("MultiCube, a new adaptation of RotatingCube, an older program of mine that improves on it in the following ways:");
            Thread.Sleep(1000);
            Console.WriteLine("\t- Lots of magic numbers floating around in the code have been put into constant fields");
            Thread.Sleep(1000);
            Console.WriteLine("\t- A huge monolith has been seperated into classes for a better overview");
            Thread.Sleep(1000);
            Console.WriteLine("\t- More comments, to better understand some of the more difficult parts of the code");
            Thread.Sleep(1000);
            Console.WriteLine("\t- Multiple cubes can be displayed at a time now and controlled seperately");
            Thread.Sleep(1000);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

            Console.Clear(); // I guarantee you, this will be the only time I am calling Console.Clear() in this program!
        }
        static List<VScreen> Init()
        {
            List<VScreen> screens = new List<VScreen>();
            Console.CursorVisible = false;
            Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;

            Console.Title = "MultiCube";
            Intro();

            SetFullscreen();

            bool end = false;
            for (int y = 0; y < (Console.WindowHeight - VSCREEN_HEIGHT + 1); y += VSCREEN_HEIGHT + 1)
            {
                int x = 0;
                // VSCREEN_* + 1 because I want to leave space for the borders of the virtual screens
                for (; x < Console.WindowWidth - VSCREEN_WIDTH + 1; x += VSCREEN_WIDTH + 1)
                {
                    screens.Add(new VScreen(VSCREEN_HEIGHT, VSCREEN_WIDTH, x, y));
                    x += VSCREEN_WIDTH;
                    for (int h = 0; h < y + VSCREEN_HEIGHT; h++)
                    {
                        Console.SetCursorPosition(x, h);
                        Console.Write('█');
                    }
                    x -= VSCREEN_WIDTH;

                    if (screens.Count == 10)
                    {
                        end = true;
                        break;
                    }
                }
                y += VSCREEN_HEIGHT;
                for (int w = 0; w < x; w++)
                {
                    Console.SetCursorPosition(w, y);
                    Console.Write('█');
                }
                y -= VSCREEN_HEIGHT;
                if (end) break;
            }
            return screens;
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
            List<VScreen> screens = Init();
            if (screens.Count < 1)
            {
                throw new AggregateException("Your screen was too small to assign virtual screens to.");
            }
            List<Cube> cubes = new List<Cube>();
            List<bool> manualControl = new List<bool>();
            List<float> angleX = new List<float>();
            List<float> angleY = new List<float>();
            List<float> angleZ = new List<float>();

            foreach (var screen in screens)
            {
                cubes.Add(
                    new Cube(
                        Math.Min(VSCREEN_HEIGHT * ZOOM_FACTOR, VSCREEN_WIDTH * ZOOM_FACTOR)
                        )
                    );
                manualControl.Add(true);
                angleX.Add(0f);
                angleY.Add(0f);
                angleZ.Add(0f);

                // print fresh cubes on all available screens
                new Cube(
                    Math.Min(VSCREEN_HEIGHT * ZOOM_FACTOR, VSCREEN_WIDTH * ZOOM_FACTOR)
                    ).Print2DProjection(0f, 0f, 0f, screen);
                screen.Refresh();
            }
            int sel = 0;
            int fheight = Console.WindowHeight;
            int fwidth = Console.WindowWidth;

            // Starting angle
            // If escape is pressed later, the program will exit
            Random random = new Random();
            bool exit = false;
            float rotationFactor = 1f;
            ConsoleKeyInfo keyPress;
            while (!exit)
            {
                // Let's keep the float values below 360f, otherwise, they don't make much sense.
                if (angleX[sel] > 360f) angleX[sel] -= 360f;
                else if (angleX[sel] < -360f) angleX[sel] += 360f;

                if (angleY[sel] > 360f) angleY[sel] -= 360f;
                else if (angleY[sel] < -360f) angleY[sel] += 360f;

                if (angleZ[sel] > 360f) angleZ[sel] -= 360f;
                else if (angleZ[sel] < -360f) angleZ[sel] += 360f;

                if (fheight == Console.WindowHeight || fwidth == Console.WindowWidth) ; // Most likely case first
                else SetFullscreen();
                //Console.SetCursorPosition(0, 0);                                                      // Debug
                //Console.Write("Height: " + Console.WindowHeight + "\tWidth: " + Console.WindowWidth); // Debug
                screens[sel].Clear();
                cubes[sel].Print2DProjection(angleX[sel], angleY[sel], angleZ[sel], screens[sel]);
                screens[sel].Refresh();
                if (Console.KeyAvailable)
                {
                    keyPress = Console.ReadKey(true);
                    switch (keyPress.Key)
                    {
                        #region Keypresses
                        case ConsoleKey.W:
                            angleX[sel] += rotationFactor;
                            break;
                        case ConsoleKey.A:
                            angleY[sel] += rotationFactor;
                            break;
                        case ConsoleKey.S:
                            angleX[sel] -= rotationFactor;
                            break;
                        case ConsoleKey.D:
                            angleY[sel] -= rotationFactor;
                            break;
                        case ConsoleKey.J:
                            angleZ[sel] += rotationFactor;
                            break;
                        case ConsoleKey.K:
                            angleZ[sel] -= rotationFactor;
                            break;
                        case ConsoleKey.R:
                            angleX[sel] = 0f;
                            angleY[sel] = 0f;
                            angleZ[sel] = 0f;
                            break;
                        case ConsoleKey.M:
                            manualControl[sel] = !manualControl[sel];
                            break;
                        case ConsoleKey.Escape:
                            exit = true;
                            break;
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            sel = 0;
                            break;
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            if (screens.Count >= 2) sel = 1;
                            break;
                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            if (screens.Count >= 3) sel = 2;
                            break;
                        case ConsoleKey.D4:
                        case ConsoleKey.NumPad4:
                            if (screens.Count >= 4) sel = 3;
                            break;
                        case ConsoleKey.D5:
                        case ConsoleKey.NumPad5:
                            if (screens.Count >= 5) sel = 4;
                            break;
                        case ConsoleKey.D6:
                        case ConsoleKey.NumPad6:
                            if (screens.Count >= 6) sel = 5;
                            break;
                        case ConsoleKey.D7:
                        case ConsoleKey.NumPad7:
                            if (screens.Count >= 7) sel = 6;
                            break;
                        case ConsoleKey.D8:
                        case ConsoleKey.NumPad8:
                            if (screens.Count >= 8) sel = 7;
                            break;
                        case ConsoleKey.D9:
                        case ConsoleKey.NumPad9:
                            if (screens.Count >= 9) sel = 8;
                            break;
                        case ConsoleKey.D0:
                        case ConsoleKey.NumPad0:
                            if (screens.Count >= 10) sel = 9;
                            break;
                            #endregion
                    }
                }
                if (manualControl[sel])
                {
                    if (Console.KeyAvailable)
                    {
                        keyPress = Console.ReadKey(true);
                        bool altDown, shiftDown;
                        altDown = (keyPress.Modifiers & ConsoleModifiers.Alt) != 0;
                        shiftDown = (keyPress.Modifiers & ConsoleModifiers.Shift) != 0;
                        if (shiftDown)
                        {
                            if (altDown) rotationFactor = 1f;
                            else rotationFactor = 0.5f;
                        }
                        else if (altDown) rotationFactor = 2f;
                        else rotationFactor = 1f;
                    }
                    else rotationFactor = 1f;
                }
                else
                {
                    switch (random.Next(1, 4))
                    {
                        case 1:
                            angleX[sel] += random.Next(0, 3);
                            break;
                        case 2:
                            angleY[sel] += random.Next(0, 3);
                            break;
                        case 3:
                            angleZ[sel] += random.Next(0, 3);
                            break;
                    }
                    Thread.Sleep(10);
                }
                Console.CursorVisible = false; // Workaround for cursor staying visible if you click into the window once
            }
        }
    }
}
