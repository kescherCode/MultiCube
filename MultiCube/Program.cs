using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiCube
{
    class Program
    {
        static readonly RegistryKey rk = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MultiCube");
        const int MAX_SCREEN_COUNT = 10;
        const float ZOOM_FACTOR = 3.2f;
        const char BORDER_CHAR = '-';

        static void Intro()
        {
            Console.WriteLine("MultiCube, a new adaptation of RotatingCube, an older program of mine that improves on it in the following ways:");
            Console.WriteLine("\t- Lots of magic numbers floating around in the code have been put into constant fields");
            Console.WriteLine("\t- A huge monolith has been seperated into classes for a better overview");
            Console.WriteLine("\t- More comments, to better understand some of the more difficult parts of the code");
            Console.WriteLine("\t- Multiple cubes can be displayed at a time now and controlled seperately");
            Console.WriteLine();
            Console.Write("Press F to ");
            string respects = "pay respects";
            string disable = "disable this message for your system globally.";

            #region Easter Egg
            for (int i = 0; i < respects.Length; i++)
            {
                Console.Write(respects[i]);
                Thread.Sleep(30);
            }
            for (int i = 0; i < respects.Length; i++)
            {
                if (Console.CursorLeft != 0)
                {
                    Console.Write("\b \b");
                }
                else
                {
                    Console.CursorTop--;
                    Console.CursorLeft = Console.WindowHeight - 1;
                    Console.Write(" \b");
                }
                Thread.Sleep(30);
            }
            for (int i = 0; i < disable.Length; i++)
            {
                Console.Write(disable[i]);
                Thread.Sleep(10);
            }
            Console.WriteLine();
            #endregion

            Console.WriteLine("After this screen, press up-down-left-right using your arrow keys to reenable this message.");
            Console.WriteLine();
            Console.WriteLine("Switch between the maximum of 10 screens using the number keys on your numpad or top row.");
            Console.WriteLine("Use W, A, S, D, J and K to rotate the cube in the selected screen manually.");
            Console.WriteLine("Press ALT at the same time to speed up the manual rotation, SHIFT to slow it down.");
            Console.WriteLine("Press M to toggle auto-rotation mode for a cube. Manual control will be disabled for that screen, but you can press M again to retain control.");
            Console.WriteLine("Press R to reset the currently selected cube. This will also disable auto-rotation mode for it.");
            Console.WriteLine("Press ESC at any time to exit this program (in fact, you can do that right now!)");
            Console.WriteLine();
            Console.WriteLine("Press any other key to just continue.");

            bool exit = false;
            while (!exit)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.F:
                        rk.SetValue("showTutorial", 0);
                        Console.WriteLine("[Registry] Tutorial disabled.");
                        break;
                    default:
                        exit = true;
                        break;
                }
            }

            Console.Clear();
        }
        static void Init(out List<VScreen> screens, out int vheight, out int vwidth)
        {
            Console.WindowHeight = 40;
            Console.WindowWidth = 142;

            Console.WriteLine("Resize the window to a size you like (ALT + Return/Enter enables fullscreen mode if your system supports it!)");
            Console.WriteLine("Press any key to continue...");
            do
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.LeftWindows || key.Key == ConsoleKey.RightWindows) continue; // In case someone wants to press WindowsKey + Up-Arrow to 
                break;
            } while (true);
            Console.Clear();

            screens = new List<VScreen>();
            Console.CursorVisible = false;

            if (rk.GetValue("showTutorial", 0).Equals(1)) Intro();
            // Dynamic virtual screen sizes
            vheight = (int)(Console.WindowHeight / 2.5);
            vwidth = (int)(Console.WindowWidth / 5.5);

            bool end = false;
            for (int y = 0; y < (Console.WindowHeight - vheight + 1); y += vheight + 1)
            {
                int x = 0;
                // VSCREEN_* + 1 since we want to leave space for the borders.
                for (; x < Console.WindowWidth - vwidth + 1; x += vwidth + 1)
                {
                    if (screens.Count != MAX_SCREEN_COUNT)
                    {
                        screens.Add(new VScreen(vheight, vwidth, x, y));

                        // Print vertical right-hand screen border for each screen
                        x += vwidth;
                        for (int h = 0; h < y + vheight; h++)
                        {
                            Console.SetCursorPosition(x, h);
                            Console.Write(BORDER_CHAR);
                        }
                        x -= vwidth;

                    }
                    else break;
                }

                // Print horizontal bottom screen row borders
                y += vheight;
                for (int w = 0; w < x; w++)
                {
                    Console.SetCursorPosition(w, y);
                    Console.Write(BORDER_CHAR);
                }
                y -= vheight;

                if (end) break;
            }
        }
        static void Main()
        {
            byte enableCombination = 0;

            Init(out List<VScreen> screens, out int vheight, out int vwidth);
            List<ScreenContainer> sc = new List<ScreenContainer>();
            foreach (VScreen screen in screens)
                sc.Add(new ScreenContainer(screen, vheight, vwidth, ZOOM_FACTOR));

            byte sel = 0;
            int fheight = Console.BufferHeight = Console.WindowHeight;
            int fwidth = Console.BufferWidth = Console.WindowWidth;

            // Starting angle
            // If escape is pressed later, the program will exit
            bool exit = false;
            float rotationFactor = 2f;
            ConsoleKeyInfo keyPress = new ConsoleKeyInfo();
            while (!exit)
            {
                // Updating the currently selected screen
                screens[sel].Clear();
                sc[sel].Cube.Update2DProjection(sc[sel].AngleX, sc[sel].AngleY, sc[sel].AngleZ, screens[sel]);

                if (Console.KeyAvailable)
                {
                    keyPress = Console.ReadKey(true);
                    sc[sel].ProcessKeypress(ref keyPress, ref rotationFactor, ref exit, ref sel, ref enableCombination);
                }

                DateTime autoStart = DateTime.Now;
                for (int i = 0; i < sc.Count; i++)
                {
                    sc[i].Autorotate();
                    if (i != sel)
                    {
                        screens[i].Clear();
                        sc[i].Cube.Update2DProjection(sc[i].AngleX, sc[i].AngleY, sc[i].AngleZ, screens[i]);
                    }
                }
                Thread.Sleep((DateTime.Now - autoStart).TotalMilliseconds < 1.1 ? 1 : 0); // We want a minimum of 1ms sleep after auto rotating the cubes.

                screens.ForEach(screen => screen.Refresh());
                Console.CursorVisible = false; // Workaround for cursor staying visible if you click into the window once
            }
        }
    }
}
