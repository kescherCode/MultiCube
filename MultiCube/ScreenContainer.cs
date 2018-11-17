using System;
using System.Diagnostics;
using System.Reflection;
using static MultiCube.Globals;

namespace MultiCube
{
    internal class ScreenContainer
    {
        public ScreenContainer(VScreen screen)
        {
            Screen = screen;
            double size = Math.Min(Screen.WindowHeight * ZoomFactor, Screen.WindowWidth * ZoomFactor);
            Cube = new Cube(size);

            // print the initial cube
            Cube.UpdateProjection(Screen);
            Screen.Refresh();
        }

        public VScreen Screen { get; }
        public Cube Cube { get; }

        // Flag for manual (true) or automatic (false) cube movement.
        public bool ManualControl { get; set; } = true;

        public void ProcessKeypress(ref ConsoleKeyInfo keyPress, ref double rotationFactor, ref bool exit, byte sel,
            ref byte enableCombination, out byte newSel)
        {
            newSel = sel;

            if (ManualControl)
            {
                // If shift is pressed, speed should be slowed down.
                // If alt is pressed, speed should be sped up.
                // If both or none are pressed, speed should be set to the default value.
                bool altDown = (keyPress.Modifiers & ConsoleModifiers.Alt) != 0;
                bool shiftDown = (keyPress.Modifiers & ConsoleModifiers.Shift) != 0;
                if (shiftDown && !altDown)
                    rotationFactor = HalfSpeed;
                else if (altDown && !shiftDown)
                    rotationFactor = DoubleSpeed;
                else rotationFactor = Speed;
            }

            switch (keyPress.Key)
            {
                case ConsoleKey.W:
                    if (ManualControl) Cube.AngleX += rotationFactor;
                    break;
                case ConsoleKey.A:
                    if (ManualControl) Cube.AngleY += rotationFactor;
                    break;
                case ConsoleKey.S:
                    if (ManualControl) Cube.AngleX -= rotationFactor;
                    break;
                case ConsoleKey.D:
                    if (ManualControl) Cube.AngleY -= rotationFactor;
                    break;
                case ConsoleKey.J:
                    if (ManualControl) Cube.AngleZ += rotationFactor;
                    break;
                case ConsoleKey.K:
                    if (ManualControl) Cube.AngleZ -= rotationFactor;
                    break;
                case ConsoleKey.R:
                    // Cube reset
                    ManualControl = true;
                    Cube.AngleX = 0;
                    Cube.AngleY = 0;
                    Cube.AngleZ = 0;
                    break;
                case ConsoleKey.M:
                    ManualControl = !ManualControl;
                    break;
                case ConsoleKey.Escape:
                    exit = true;
                    break;
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    newSel = 0;
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    newSel = 1;
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    newSel = 2;
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    newSel = 3;
                    break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    newSel = 4;
                    break;
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    newSel = 5;
                    break;
                case ConsoleKey.D7:
                case ConsoleKey.NumPad7:
                    newSel = 6;
                    break;
                case ConsoleKey.D8:
                case ConsoleKey.NumPad8:
                    newSel = 7;
                    break;
                case ConsoleKey.D9:
                case ConsoleKey.NumPad9:
                    newSel = 8;
                    break;
                case ConsoleKey.D0:
                case ConsoleKey.NumPad0:
                    newSel = 9;
                    break;
                case ConsoleKey.UpArrow:
                    enableCombination = enableCombination == 0 ? (byte) 1 : (byte) 0;
                    break;
                case ConsoleKey.DownArrow:
                    enableCombination = enableCombination == 1 ? (byte) 2 : (byte) 0;
                    break;
                case ConsoleKey.LeftArrow:
                    enableCombination = enableCombination == 2 ? (byte) 3 : (byte) 0;
                    break;
                case ConsoleKey.RightArrow:
                    if (enableCombination == 3) enableCombination = 0;

                    RegistrySettings.ShowTutorial = true;
                    lock (ConsoleLock)
                    {
                        Console.SetCursorPosition(0, Console.WindowHeight - 1);
                        Console.Write("[Registry] Tutorial enabled!");
                    }

                    break;
                case ConsoleKey.OemPeriod:
                    // Start a new process using the path the current .exe was started from and exit the current process.
                    Process.Start(Assembly.GetExecutingAssembly().Location,
                        $"{Console.WindowHeight} {Console.WindowWidth} true");
                    Environment.Exit(0);
                    break;
                default:
                    enableCombination = 0;
                    break;
            }
        }

        /// <summary>
        ///     Provides one tick of automatic rotation to all cubes that are set to autorotation mode.
        /// </summary>
        public void Autorotate()
        {
            if (ManualControl) return;
            Cube.AngleX += Globals.Random.NextDouble();
            Cube.AngleY += Globals.Random.NextDouble();
            Cube.AngleZ += Globals.Random.NextDouble();
        }
    }
}