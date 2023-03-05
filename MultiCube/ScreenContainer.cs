using System;
using System.Reflection;
using static System.ConsoleKey;
using static System.ConsoleModifiers;
using static System.Math;
using static MultiCube.Globals;
using static MultiCube.Settings;
using Process = System.Diagnostics.Process;

namespace MultiCube
{
    /// <summary>
    ///     A class that combines a Screen and a Cube in one.
    /// </summary>
    internal class ScreenContainer
    {
        /// <summary>
        ///     Initializes a ScreenContainer instance that creates a new Cube for the given screen.
        /// </summary>
        /// <param name="screen">VScreen that needs a Cube</param>
        public ScreenContainer(VScreen screen)
        {
            Screen = screen;
            Cube = new Cube(Min(Screen.WindowHeight, Screen.WindowWidth));

            // print the initial cube
            Cube.ProjectToVScreen(Screen);
            Screen.Output();
        }

        public VScreen Screen { get; set; }
        public Cube Cube { get; }

        // Flag for manual (true) or automatic (false) cube movement.
        private bool ManualControl { get; set; } = true;

        /// <summary>
        ///     Processes a keypress.
        /// </summary>
        /// <param name="keyPress">The keypress reference.</param>
        /// <param name="rotationFactor">The rotationFactor reference.</param>
        /// <param name="exit">The reference to the bool that indicates whether the program should exit or not.</param>
        /// <param name="sel">The zero-based current screen selection.</param>
        /// <param name="enableCombination">The byte that indicates the current zero-based stage of re-enabling the intro.</param>
        /// <param name="newSel">The new screen selection. Could be the same as sel.</param>
        public void ProcessKeypress(ref ConsoleKeyInfo keyPress, ref double rotationFactor, ref bool exit, byte sel,
            ref byte enableCombination, out byte newSel)
        {
            newSel = sel;

            #region Keypresses

            if (ManualControl)
            {
                // If shift is pressed, speed should be slowed down.
                // If alt is pressed, speed should be sped up.
                // If both or none are pressed, speed should be set to the default value.
                var altDown = (keyPress.Modifiers & Alt) != 0;
                var shiftDown = (keyPress.Modifiers & Shift) != 0;
                if (shiftDown && !altDown)
                    rotationFactor = HalfFactor;
                else if (altDown && !shiftDown)
                    rotationFactor = DoubleFactor;
                else rotationFactor = NormalFactor;
            }

            if (keyPress.Key is not UpArrow and not DownArrow and not LeftArrow and not RightArrow)
            {
                enableCombination = 0;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (keyPress.Key)
            {
                case W:
                    if (ManualControl) Cube.AngleX += rotationFactor;
                    break;
                case A:
                    if (ManualControl) Cube.AngleY += rotationFactor;
                    break;
                case S:
                    if (ManualControl) Cube.AngleX -= rotationFactor;
                    break;
                case D:
                    if (ManualControl) Cube.AngleY -= rotationFactor;
                    break;
                case J:
                    if (ManualControl) Cube.AngleZ += rotationFactor;
                    break;
                case K:
                    if (ManualControl) Cube.AngleZ -= rotationFactor;
                    break;
                case R:
                    // Cube reset
                    ManualControl = true;
                    Cube.AngleX = 0;
                    Cube.AngleY = 0;
                    Cube.AngleZ = 0;
                    break;
                case M:
                    ManualControl = !ManualControl;
                    break;
                case Escape:
                    exit = true;
                    break;
                case D1:
                case NumPad1:
                    newSel = 0;
                    break;
                case D2:
                case NumPad2:
                    newSel = 1;
                    break;
                case D3:
                case NumPad3:
                    newSel = 2;
                    break;
                case D4:
                case NumPad4:
                    newSel = 3;
                    break;
                case D5:
                case NumPad5:
                    newSel = 4;
                    break;
                case D6:
                case NumPad6:
                    newSel = 5;
                    break;
                case D7:
                case NumPad7:
                    newSel = 6;
                    break;
                case D8:
                case NumPad8:
                    newSel = 7;
                    break;
                case D9:
                case NumPad9:
                    newSel = 8;
                    break;
                case D0:
                case NumPad0:
                    newSel = 9;
                    break;
                case UpArrow:
                    enableCombination = enableCombination == 0 ? (byte) 1 : (byte) 0;
                    break;
                case DownArrow:
                    enableCombination = enableCombination == 1 ? (byte) 2 : (byte) 0;
                    break;
                case LeftArrow:
                    enableCombination = enableCombination == 2 ? (byte) 3 : (byte) 0;
                    break;
                case RightArrow:
                    if (enableCombination == 3)
                    {
                        enableCombination = 0;

                        ShowIntro = true;
                        lock (ConsoleLock)
                        {
                            Console.SetCursorPosition(0, Console.WindowHeight - 1);
                            Console.Write("[Config] Tutorial enabled!");
                        }
                    }

                    break;
                default:
                    break;

                #endregion
            }
        }

        /// <summary>
        ///     Provides one tick of automatic rotation to all cubes that are set to autorotation mode.
        /// </summary>
        public void Autorotate()
        {
            if (ManualControl) return;

            Cube.AngleX += Globals.Random.NextDouble() * NormalFactor;
            Cube.AngleY += Globals.Random.NextDouble() * NormalFactor;
            Cube.AngleZ += Globals.Random.NextDouble() * NormalFactor;
        }
    }
}