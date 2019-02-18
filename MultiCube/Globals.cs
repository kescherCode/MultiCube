using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace MultiCube
{
    /// <summary>
    ///     Stores "global" variables.
    /// </summary>
    internal struct Globals
    {
        // Object used for locking and synchronizing access to console.
        public static readonly object ConsoleLock = new object();

        // The maximum amount of screens that should exist.
        public const int ScreenCount = 10;

        // Char used for a cube's foreground.
        public const char CubeCharFG = 'o';

        // Char used for a cube's background.
        public const char CubeCharBG = '-';

        // Different rotation factors
        public const double NormalFactor = 6d, DoubleFactor = NormalFactor * 2d, HalfFactor = NormalFactor / 2d;

        // We only need one PRNG.
        public static readonly Random Random = new Random(10000);

        public static void LogException(Exception e)
        {
            Console.CursorTop = Console.CursorLeft = 0;
            Console.WriteLine("An unknown exception has occured.");

            #region Exception logging

            string logPath =
                $"{Path.GetTempPath()}/MultiCube-Exception-{DateTime.Now.Ticks}.log";
            try
            {
                using (var w = new StreamWriter(logPath, false, Encoding.UTF8))
                {
                    bool done;
                    Console.WriteLine(
                        $"A log is going to be written to {logPath}. Please send this to the developer, with a description of what you were trying to do!");
                    string name = Assembly.GetExecutingAssembly().GetName().FullName;
                    w.WriteLine($"{name}");
                    do
                    {
                        w.WriteLine(e.StackTrace);
                        w.WriteLine(e.Message);
                        w.WriteLine(e.Source);
                        w.WriteLine(e.TargetSite);
                        w.WriteLine(Environment.CommandLine);
                        w.WriteLine($"64bit OS: {Environment.Is64BitOperatingSystem}");
                        w.WriteLine($"64bit process: {Environment.Is64BitProcess}");
                        w.WriteLine(Environment.OSVersion);
                        done = null == e.InnerException;
                        if (!done) e = e.InnerException;
                        w.WriteLine();
                    } while (!done);
                }
            }
            catch (Exception)
            {
                // Probably unwritable path. Let's not write a log in that case.
            }

            Environment.Exit(1);

            #endregion
        }
    }
}