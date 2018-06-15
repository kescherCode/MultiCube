using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace MultiCube
{
    class ScreenContainer
    {
        const float SPEED = 5f, DOUBLE_SPEED = 10f, HALF_SPEED = 2.5f; // User control speeds
        const int AUTO_SPEED = 5; // Auto rotation speed - 1

        private static Random random = new Random();

        public VScreen Screen { get; }
        public Cube Cube { get; }

        // Flag for manual (true) or automatic (false) cube movement.
        public bool ManualControl { get; set; } = true;

        public ScreenContainer(VScreen screen, float ZOOM_FACTOR)
        {
            Screen = screen;
            float size = Math.Min(Screen.WindowHeight * ZOOM_FACTOR, Screen.WindowWidth * ZOOM_FACTOR);
            int ledgeLength = (int) (size / (ZOOM_FACTOR));
            Cube = new Cube(size, ledgeLength);

            // print the cube
            Cube.Update2DProjection(Screen);
            Screen.Refresh();
        }

        public void ProcessKeypress(ref ConsoleKeyInfo keyPress, ref float rotationFactor, ref bool exit, byte sel, ref byte enableCombination, out byte newSel)
        {
            newSel = sel;
            #region Keypresses
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
                    ManualControl = true;
                    Cube.AngleX = 0f;
                    Cube.AngleY = 0f;
                    Cube.AngleZ = 0f;
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
                    if (enableCombination == 0) enableCombination = 1;
                    else enableCombination = 0;
                    break;
                case ConsoleKey.DownArrow:
                    if (enableCombination == 1) enableCombination = 2;
                    else enableCombination = 0;
                    break;
                case ConsoleKey.LeftArrow:
                    if (enableCombination == 2) enableCombination = 3;
                    else enableCombination = 0;
                    break;
                case ConsoleKey.RightArrow:
                    if (enableCombination == 3) enableCombination = 0;

                    RegistrySettings.ShowTutorial = true;
                    Console.SetCursorPosition(0, Console.WindowHeight - 1);
                    Console.Write("[Registry] Tutorial enabled!");
                    break;
                default:
                    enableCombination = 0;
                    break;
                    #endregion
            }

            if (ManualControl)
            {
                bool altDown, shiftDown;
                altDown = (keyPress.Modifiers & ConsoleModifiers.Alt) != 0;
                shiftDown = (keyPress.Modifiers & ConsoleModifiers.Shift) != 0;
                if (shiftDown)
                {
                    if (altDown) rotationFactor = AUTO_SPEED;
                    else rotationFactor = HALF_SPEED;
                }
                else if (altDown) rotationFactor = DOUBLE_SPEED;
                else rotationFactor = AUTO_SPEED;
            }
        }

        public void Autorotate()
        {
            if (!ManualControl)
            {
                switch (random.Next(1, 4))
                {
                    case 1:
                        Cube.AngleX += random.Next(0, AUTO_SPEED);
                        break;
                    case 2:
                        Cube.AngleY += random.Next(0, AUTO_SPEED);
                        break;
                    case 3:
                        Cube.AngleZ += random.Next(0, AUTO_SPEED);
                        break;
                }
            }
        }
    }
}
