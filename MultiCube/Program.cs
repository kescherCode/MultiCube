using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static MultiCube.Globals;

namespace MultiCube
{
    struct Program
    {
        // Minimum number of ticks the execution loop should run
        static readonly TimeSpan minTime = new TimeSpan(320000);
        // Fault tolerance for above
        static readonly TimeSpan tolTime = new TimeSpan(330000);
        /// <summary>
        /// Gives an intro to the user about using the program.
        /// </summary>
        static void Intro()
        {
            lock (consoleLock)
            {
                Console.Write("Press F to ");
                string respects = "pay respects";
                string disable = "disable this message for your user account.";

                #region Easter Egg... kinda
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
                Console.WriteLine("Switch between the 10 screens using the number keys on your numpad or top row.");
                Console.WriteLine("Use W, A, S, D, J and K to rotate the cube in the selected screen manually.");
                Console.WriteLine("Press ALT at the same time to speed up the manual rotation, SHIFT to slow it down.");
                Console.WriteLine("Press M to toggle auto-rotation mode for a cube. Manual control will be disabled for that screen, but you can press M again to regain control.");
                Console.WriteLine("Press R to reset the currently selected cube. This will also disable auto-rotation mode for it.");
                Console.WriteLine("Press ESC at any time to exit this program (in fact, you can do that right now!)");
                Console.WriteLine("Press the . (period, dot) key to open a new instance of the program and end the current one (basically a restart, but not technically)");
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
        }

        /// <summary>
        /// Do some initializing work (mainly environment prep).
        /// </summary>
        /// <param name="screens">List containing the VScreen instances</param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="skipResize"></param>
        static void Init(out List<VScreen> screens, int height, int width, bool skipResize)
        {
            // If height and width are not too small or too large, set them from input parameters.
            if (height <= Console.LargestWindowHeight &&
                width <= Console.LargestWindowWidth &&
                height > 1 && width > 1)
            {
                Console.WindowHeight = height;
                Console.WindowWidth = width;
            }
            // Otherwise, set default values and disable resize skipping..
            else
            {
                height = width = (int)(Console.LargestWindowHeight / 1.3);
                skipResize = false;
            }

            if (!skipResize)
            {
                lock (consoleLock)
                {
                    Console.WriteLine("Resize the window to a size you like and press any key.");
                    Console.ReadKey(true);
                }
            }

            // If launched from command line, we don't want to have any garbage from that left on the screen
            Console.Clear();

            screens = new List<VScreen>();

            if (RegistrySettings.ShowTutorial) Intro();
            lock (consoleLock)
            {
                // Virtual screen sizes
                int vheight = (int)(Console.WindowHeight / 2.5);
                int vwidth = (int)(Console.WindowWidth / 5.5);
                // v?Border = size with space we are leaving for the borders
                int vhBorder = vheight + 1;
                int vwBorder = vwidth + 1;

                // All screens need to have a space in-between for the border.
                for (int yOffset = 0; yOffset < Console.WindowHeight - vhBorder; yOffset += vhBorder)
                {
                    int xOffset = 0;
                    for (; xOffset < Console.WindowWidth - vwBorder; xOffset += vwBorder)
                    {
                        if (screens.Count != SCREEN_COUNT)
                        {
                            VScreen screen = new VScreen(vwidth, vheight, xOffset, yOffset);
                            screens.Add(screen);
                            screen.PrintBorders();
                        }
                        else break;
                    }
                }
            }
        }

        /// <summary>
        /// Starting point of this application.
        /// </summary>
        /// <param name="args">Commandline arguments</param>
        static void Main(string[] args)
        {
            // Guaranteed to be set to non-null value later on.
            List<VScreen> screens = null;

            /* args: MultiCube.exe (<height> <width>) <skipResize> 
               <height> console window height in characters : a positive integer
               <width> console window width in characters : a positive integer
               <skipResize> skip the resizing prompt : ("true"/"false")
               All parameters are optional, though if you provide height or width,
               you must provide both of them.
             */

            // If there are at least two valid numbers given as argument...
            if (args.Length > 1 && int.TryParse(args[0], out int height) && int.TryParse(args[1], out int width))
                // ...then check if there is a third argument that is either "true" or "false"...
                if (args.Length > 2 && bool.TryParse(args[2], out bool skipResize))
                    // If the third argument exists and is valid, pass it to Init()...
                    Init(out screens, height, width, skipResize);
                else
                    // Else, ignore the bool
                    Init(out screens, height, width, false);
            // If the numbers aren't valid or given, defaults will be passed into Init().
            else
                Init(out screens, (int)(Console.LargestWindowHeight / 1.3), (int)(Console.LargestWindowWidth / 1.3), false);

            #region Cursor being visible workaround
            // Every second, a background task disables the cursor to workaround a bug in the windows console that causes it to become visible again.
            void cursorFix(Task f) { Console.CursorVisible = false; Task.Delay(1000).ContinueWith(cursorFix); }
            new Task(() => Task.Delay(0).ContinueWith(cursorFix)).Start();
            #endregion

            List<ScreenContainer> sc = new List<ScreenContainer>();

            // Create a new ScreenContainer instance for each 
            foreach (VScreen screen in screens)
                sc.Add(new ScreenContainer(screen));
            screens.Clear();

            byte sel = 0;

            sc[sel].Screen.PrintBorders(ConsoleColor.Green); // Marks the currently selected screen

            int fheight = Console.BufferHeight = Console.WindowHeight;
            int fwidth = Console.BufferWidth = Console.WindowWidth;

            // If escape is pressed later, the program will exit
            bool exit = false;
            // The amount of times the main loop code should run until the result is put out.
            const int runsBeforeOutput = 5;
            // factor by which a cube is rotated into a direction. Will be passed by into ScreenContainer.ProcessKeypress() later
            double rotationFactor = SPEED;
            // A counter for renabling the intro if it was disabled. Look at ScreenContainer.ProcessKeypress() to see how it's used
            byte enableCombination = 0;
            ConsoleKeyInfo keyPress = new ConsoleKeyInfo();
            // Used for checking how fast the while loop below was executed.
            Stopwatch watch = new Stopwatch();
            while (!exit)
            {
                watch.Restart();
                // We run the program multiple times before output because it seems smoother to users.
                for (int runs = 0; runs != runsBeforeOutput; runs++)
                {
                    Parallel.For(0, sc.Count, i =>
                        sc[i].Autorotate());

                    if (Console.KeyAvailable)
                    {
                        keyPress = Console.ReadKey(true);
                        sc[sel].ProcessKeypress(ref keyPress, ref rotationFactor, ref exit, sel, ref enableCombination, out byte newSel);

                        // Giving a hint to the IL compiler's optimization routines. (Most likely case first)
                        if (newSel == sel) { }
                        else if (newSel < sc.Count)
                        {
                            sc[sel].Screen.PrintBorders(Console.ForegroundColor);
                            sel = newSel;
                            sc[sel].Screen.PrintBorders(ConsoleColor.Green);
                        }
                    }
                }

                Parallel.For(0, sc.Count, i =>
                {
                    // We don't want to draw a cube atop the previous projection so we have to clear the screen, so we clear it beforehand
                    sc[i].Screen.Clear();
                    sc[i].Cube.UpdateProjection(sc[i].Screen);
                });

                // Refreshing all screens one after another is faster than doing so in parallel due to console locking.
                for (int i = 0; i < sc.Count; i++)
                    sc[i].Screen.Refresh();

                watch.Stop();
                // Tolerance increases consistency.
                if (tolTime > watch.Elapsed)
                    // A little Thread.Sleep() gives the CPU some pause and keeps cube rotation speeds (relatively) consistent :)
                    Thread.Sleep(minTime - watch.Elapsed);
            }
        }
    }
}
