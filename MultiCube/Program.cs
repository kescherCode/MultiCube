using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using static System.ConsoleKey;
using static MultiCube.Globals;

namespace MultiCube
{
    internal struct Program
    {
        private const int ProcessTime = 16;

        private const string Usage = "Usage: MultiCube.exe <skipResize>\n" +
                                     "<skipResize> skip the resizing prompt: (true/false). Default: false";

        /// <summary>
        ///     Gives an intro to the user about using the program.
        /// </summary>
        private static void Intro()
        {
            lock (ConsoleLock)
            {
                Console.Write("Press F to ");
                const string respects = "pay respects";
                const string disable = "disable this message for your user account.";

                #region Easter Egg... kinda

                foreach (char c in respects)
                {
                    Console.Write(c);
                    Thread.Sleep(30);
                }

                Thread.Sleep(300);
                for (int i = 0; i < respects.Length; ++i)
                {
                    if (Console.CursorLeft != 0)
                    {
                        Console.Write("\b \b");
                    }
                    else
                    {
                        --Console.CursorTop;
                        Console.CursorLeft = Console.WindowHeight - 1;
                        Console.Write(" \b");
                    }

                    Thread.Sleep(30);
                }

                foreach (char c in disable)
                {
                    Console.Write(c);
                    Thread.Sleep(10);
                }

                Console.WriteLine();

                #endregion

                Console.WriteLine(
                    "After this screen, press up-down-left-right using your arrow keys to reenable this message.");
                Console.WriteLine();
                Console.WriteLine("Switch between the 10 screens using the number keys on your numpad or top row.");
                Console.WriteLine("Use W, A, S, D, J and K to rotate the cube in the selected screen manually.");
                Console.WriteLine("Press ALT at the same time to speed up the manual rotation, SHIFT to slow it down.");
                Console.WriteLine(
                    "Press M to toggle auto-rotation mode for a cube. Manual control will be disabled for that screen, but you can press M again to regain control.");
                Console.WriteLine(
                    "Press R to reset the currently selected cube. This will also disable auto-rotation mode for it.");
                Console.WriteLine("Press ESC at any time to exit this program (in fact, you can do that right now!)");
                Console.WriteLine(
                    "Press the . (period, dot) key to open a new instance of the program and end the current one (basically a restart, but not technically)");
                Console.WriteLine();
                Console.WriteLine("Press any other key to just continue.");

                bool exit = false;
                while (!exit)
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (Console.ReadKey(true).Key)
                    {
                        case Escape:
                            Environment.Exit(0);
                            break;
                        case F:
                            Settings.ShowIntro = false;
                            Console.WriteLine("[Registry] Tutorial disabled.");
                            break;
                        default:
                            exit = true;
                            break;
                    }

                Console.Clear();
            }
        }

        /// <summary>
        ///     Do some initializing work (mainly environment prep).
        /// </summary>
        /// <param name="screens">List containing the VScreen instances</param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="skipResize"></param>
        private static void Init(out List<VScreen> screens, bool skipResize)
        {
            Console.Title = "MultiCube";


            // Considering the possibility of launching from command line, we don't want to have any residual output from it left on the screen.
            Console.Clear();

            if (!skipResize)
                lock (ConsoleLock)
                {
                    ConsoleKeyInfo key;
                    Console.WriteLine("Resize the window to a size you like and press any key.");
                    do
                    {
                        key = Console.ReadKey(true);
                    } while (key.Key == LeftWindows || key.Key == RightWindows);
                }

            screens = new List<VScreen>();

            if (Settings.ShowIntro) Intro();
            lock (ConsoleLock)
            {
                // Virtual screen sizes. They were found by trial and error.
                int vheight = (int) (Console.WindowHeight / 2.5);
                int vwidth = (int) (Console.WindowWidth / 5.5);
                // v?Border = size with space we are leaving for the borders
                int vhBorder = vheight + 1;
                int vwBorder = vwidth + 1;

                // All screens need to have a space in-between for the border.
                for (int yOffset = 0; yOffset < Console.WindowHeight - vhBorder; yOffset += vhBorder)
                {
                    int xOffset = 0;
                    for (; xOffset < Console.WindowWidth - vwBorder; xOffset += vwBorder)
                        if (screens.Count != ScreenCount)
                        {
                            var screen = new VScreen(vwidth, vheight, xOffset, yOffset);
                            screens.Add(screen);
                            screen.PrintBorders();
                        }
                        else
                        {
                            break;
                        }
                }
            }
        }

        /// <summary>
        ///     Starting point of this application.
        /// </summary>
        /// <param name="args">Commandline arguments</param>
        private static void Main(string[] args)
        {
            if (!Environment.UserInteractive) return;

            // Guaranteed to be set to non-null value later on.
            List<VScreen> screens;

            /* Usage: MultiCube.exe <skipResize>
               <skipResize> skip the resizing prompt: ("true"/"false"). Default: false
             */

            if (args.Length >= 1 && args[0].Contains("help"))
            {
                Console.WriteLine(Usage);
                return;
            }
            
                // ...then check if there is an argument that is either true or false...
                if (args.Length >= 1 && bool.TryParse(args[0], out bool skipResize))
                    // If the third argument exists and is valid, pass it to Init()...
                    Init(out screens, skipResize);
                // else init with default values
                else Init(out screens, false);

            #region Cursor being visible workaround

            // Every second, a background task disables the cursor to workaround an oversight in the windows console that sometimes causes it to become visible again.
            void CursorFix(Task f)
            {
                Console.CursorVisible = false;
                Task.Delay(1000).ContinueWith(CursorFix);
            }

            new Task(() => Task.Delay(0).ContinueWith(CursorFix)).Start();

            #endregion

            var sc = new List<ScreenContainer>();

            // Create a new ScreenContainer instance for each screen-cube-pair.
            foreach (VScreen screen in screens)
                sc.Add(new ScreenContainer(screen));
            screens.Clear();

            byte sel = 0;

            sc[sel].Screen.PrintBorders(color: ConsoleColor.Green); // Marks the currently selected screen

            // If escape is pressed later, the program will exit
            bool exit = false;
            // factor by which a cube is rotated into a direction. Will be passed by into ScreenContainer.ProcessKeypress() later
            double rotationFactor = NormalFactor;
            // A counter for re-enabling the intro if it was disabled. Look at ScreenContainer.ProcessKeypress() to see how it's used
            byte enableCombination = 0;
#if DEBUG
            var fps = 0;

            #region Print frames per second

            // ReSharper disable once ImplicitlyCapturedClosure
            void PrintFps(Task f)
            {
                Console.Title = $"{fps} fps";
                fps = 0;
                Task.Delay(1000).ContinueWith(PrintFps);
            }

            new Task(() => Task.Delay(1000).ContinueWith(PrintFps)).Start();

            #endregion

#endif

            async Task Input()
            {
                await Task.Run(() =>
                {
                    if (!Console.KeyAvailable) return;
                    ConsoleKeyInfo keyPress = Console.ReadKey(true);

                    sc[sel].ProcessKeypress(ref keyPress, ref rotationFactor, ref exit, sel,
                        ref enableCombination,
                        out byte newSel);

                    if (sel == newSel || newSel >= sc.Count) return;
                    sc[sel].Screen.PrintBorders(color: Console.ForegroundColor);
                    sel = newSel;
                    sc[sel].Screen.PrintBorders(color: ConsoleColor.Green);
                });
            }

            async Task Autorotate()
            {
                await Task.WhenAll(
                    Task.Run(
                        () =>
                        {
                            foreach (ScreenContainer c in sc)
                                c.Autorotate();
                        }),
                    Task.Delay(ProcessTime));
            }

            async Task Output()
            {
                await Task.Run(() =>
                {
                    // Outputting all screens one after another is faster than doing so in parallel due to console locking.
                    foreach (ScreenContainer c in sc)
                    {
                        // We don't want to draw a cube atop the previous projection so we have to clear the screen, so we clear it beforehand
                        c.Screen.Clear();
                        c.Cube.ProjectToVScreen(c.Screen);
                        c.Screen.Output();
                    }
                });
#if DEBUG
                ++fps;
#endif
            }

            // Less aggressive GC.
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            Task input = Task.CompletedTask;
            Task autorotate = Task.CompletedTask;
            Task output = Task.CompletedTask;
            try
            {
                while (!exit)
                {
                    // Reduces CPU load
                    // ReSharper disable AccessToModifiedClosure
                    // ReSharper disable AccessToDisposedClosure
                    SpinWait.SpinUntil(() => input.IsCompleted || autorotate.IsCompleted || output.IsCompleted);
                    // ReSharper restore AccessToModifiedClosure
                    // ReSharper restore AccessToDisposedClosure

                    if (input.IsCompleted)
                    {
                        if (!input.IsFaulted)
                        {
                            input.Dispose();
                            input = Input();
                        }
                        else if (input.Exception != null)
                        {
                            AggregateException ex = input.Exception;
                            input.Dispose();
                            throw ex;
                        }
                    }

                    if (autorotate.IsCompleted)
                    {
                        if (!input.IsFaulted)
                        {
                            autorotate.Dispose();
                            autorotate = Autorotate();
                        }
                        else if (autorotate.Exception != null)
                        {
                            AggregateException ex = autorotate.Exception;
                            autorotate.Dispose();
                            throw ex;
                        }
                    }

                    if (!output.IsCompleted) continue;

                    if (!output.IsFaulted)
                    {
                        output.Dispose();
                        output = Output();
                    }
                    else if (output.Exception != null)
                    {
                        AggregateException ex = output.Exception;
                        output.Dispose();
                        throw ex;
                    }
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
    }
}