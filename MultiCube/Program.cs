using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MultiCube
{
    class Program
    {
        static readonly TimeSpan minTime = new TimeSpan(50000);

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
            string disable = "disable this message for your user account.";

            #region Easter Egg
            for (int i = 0; i < respects.Length; i++)
            {
                Console.Write(respects[i]);
                Thread.Sleep(30);
            }
            Thread.Sleep(300);
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
                        RegistrySettings.ShowTutorial = false;
                        Console.WriteLine("[Registry] Tutorial disabled.");
                        break;
                    default:
                        exit = true;
                        break;
                }
            }

            Console.Clear();
        }
        static void Init(out List<VScreen> screens)
        {
            Console.WindowHeight = 40;
            Console.WindowWidth = 142;
            Console.WriteLine("Resize the window to a size you like");
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

            if (RegistrySettings.ShowTutorial) Intro();
            // Dynamic virtual screen sizes
            int vheight = (int)(Console.WindowHeight / 2.5);
            int vwidth = (int)(Console.WindowWidth / 5.5);

            for (int y = 0; y < (Console.WindowHeight - vheight + 1); y += vheight + 1)
            {
                int x = 0;
                // height or width + 1 since we want to leave space for the borders.
                for (; x < Console.WindowWidth - vwidth + 1; x += vwidth + 1)
                {
                    if (screens.Count != Globals.MAX_SCREEN_COUNT)
                    {
                        VScreen screen = new VScreen(vheight, vwidth, x, y);
                        screens.Add(screen);
                        screen.PrintBorders(Console.ForegroundColor);
                    }
                    else break;
                }
            }
        }
        static void Main()
        {
            Stopwatch watch = new Stopwatch();
            byte enableCombination = 0;

            Init(out List<VScreen> screens);

            List<ScreenContainer> sc = new List<ScreenContainer>();
            foreach (VScreen screen in screens)
                sc.Add(new ScreenContainer(screen));
            screens.Clear();

            byte sel = 0;

            sc[sel].Screen.PrintBorders(ConsoleColor.Green); // Marks the currently selected screen

            int fheight = Console.BufferHeight = Console.WindowHeight;
            int fwidth = Console.BufferWidth = Console.WindowWidth;

            // Starting angle
            // If escape is pressed later, the program will exit
            bool exit = false;
            float rotationFactor = 2f;
            ConsoleKeyInfo keyPress = new ConsoleKeyInfo();
            while (!exit)
            {
                watch.Restart();
                // Updating the currently selected screen
                sc[sel].Screen.Clear();
                sc[sel].Cube.Update2DProjection(sc[sel].Screen);

                if (Console.KeyAvailable)
                {
                    keyPress = Console.ReadKey(true);
                    sc[sel].ProcessKeypress(ref keyPress, ref rotationFactor, ref exit, sel, ref enableCombination, out byte newSel);
                    if (newSel != sel)
                    {
                        sc[sel].Screen.PrintBorders(Console.ForegroundColor);
                        sel = newSel;
                        sc[sel].Screen.PrintBorders(ConsoleColor.Green);
                    }
                }

                for (int i = 0; i < sc.Count; i++)
                {
                    sc[i].Autorotate();
                    if (i != sel)
                    {
                        sc[i].Screen.Clear();
                        sc[i].Cube.Update2DProjection(sc[i].Screen);
                    }
                }
                watch.Stop();

                if (minTime > watch.Elapsed) Thread.Sleep(minTime - watch.Elapsed);
                sc.ForEach(c => c.Screen.Refresh());

                Console.CursorVisible = false; // Workaround for cursor being visible if you click into the window once
            }
        }
    }
}
