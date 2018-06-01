using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace MultiCube
{
    class ScreenContainer
    {
        static readonly RegistryKey rk = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MultiCube");
        const float SPEED = 5f, DOUBLE_SPEED = 10f, HALF_SPEED = 2.5f; // User control speeds
        const int AUTO_SPEED = 5; // Auto rotation speed - 1

        private static Random random = new Random();

        public VScreen Screen { get; }
        public Cube Cube { get; }

        // Flag for manual (true) or automatic (false) cube movement.
        public bool ManualControl { get; set; } = true;
        // "Camera position" for the cubes
        public float AngleX { get; set; } = 0f;
        public float AngleY { get; set; } = 0f;
        public float AngleZ { get; set; } = 0f;

        public ScreenContainer(VScreen screen, int vheight, int vwidth, float ZOOM_FACTOR)
        {
            Screen = screen;

            Cube = new Cube(
                    Math.Min(vheight * ZOOM_FACTOR, vwidth * ZOOM_FACTOR)
                    );

            // print the cube
            Cube.Update2DProjection(0f, 0f, 0f, screen);
            screen.Refresh();
        }

        public void ProcessKeypress(ref ConsoleKeyInfo keyPress, ref float rotationFactor, ref bool exit, ref byte sel, ref byte enableCombination)
        {
            #region Keypresses
            switch (keyPress.Key)
            {
                case ConsoleKey.W:
                    if (ManualControl) AngleX += rotationFactor;
                    break;
                case ConsoleKey.A:
                    if (ManualControl) AngleY += rotationFactor;
                    break;
                case ConsoleKey.S:
                    if (ManualControl) AngleX -= rotationFactor;
                    break;
                case ConsoleKey.D:
                    if (ManualControl) AngleY -= rotationFactor;
                    break;
                case ConsoleKey.J:
                    if (ManualControl) AngleZ += rotationFactor;
                    break;
                case ConsoleKey.K:
                    if (ManualControl) AngleZ -= rotationFactor;
                    break;
                case ConsoleKey.R:
                    ManualControl = true;
                    AngleX = 0f;
                    AngleY = 0f;
                    AngleZ = 0f;
                    break;
                case ConsoleKey.M:
                    ManualControl = !ManualControl;
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
                    sel = 1;
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    sel = 2;
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    sel = 3;
                    break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    sel = 4;
                    break;
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    sel = 5;
                    break;
                case ConsoleKey.D7:
                case ConsoleKey.NumPad7:
                    sel = 6;
                    break;
                case ConsoleKey.D8:
                case ConsoleKey.NumPad8:
                    sel = 7;
                    break;
                case ConsoleKey.D9:
                case ConsoleKey.NumPad9:
                    sel = 8;
                    break;
                case ConsoleKey.D0:
                case ConsoleKey.NumPad0:
                    sel = 9;
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

                    rk.SetValue("showTutorial", 1);
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
                        AngleX += random.Next(0, AUTO_SPEED);
                        break;
                    case 2:
                        AngleY += random.Next(0, AUTO_SPEED);
                        break;
                    case 3:
                        AngleZ += random.Next(0, AUTO_SPEED);
                        break;
                }
            }
        }
    }
}
