using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MultiCube.Globals;

namespace MultiCube
{
    /// <summary>
    ///     A class that stores a cube and allows 2D projection into a VScreen class.
    /// </summary>
    internal class Cube
    {
        // These constants "magnify" the view field and divide the total of coord, screen width and viewFactor.
        private const double ViewFactor = 2.5f, PointDivisor = 5;

        // All possible corners on a cube
        private static readonly List<Scalar3D> Corners = new List<Scalar3D>
        {
            new Scalar3D(-1, -1, -1),
            new Scalar3D(1, -1, -1),
            new Scalar3D(1, -1, 1),
            new Scalar3D(-1, -1, 1),
            new Scalar3D(-1, 1, 1),
            new Scalar3D(-1, 1, -1),
            new Scalar3D(1, 1, -1),
            new Scalar3D(1, 1, 1)
        };

        // A LINQ query that puts all valid corner coordinates into a collection of LineData
        private static readonly IEnumerable<LineData> Lines = (
            from a in Corners
            from b in Corners
            where Math.Abs((a - b).Length - 2) < double.Epsilon && a.X + a.Y + a.Z > b.X + b.Y + b.Z
            select new LineData(a, b)
        ).ToList();

        // _ledgeStep is _ledgeLength
        private readonly int _ledgeStep;
        private readonly double _size, _fov, _ledgeLength;

        private double _angleX, _angleY, _angleZ;

        /// <summary>
        ///     Initializes a new Cube instanze.
        /// </summary>
        /// <param name="size">Cube size</param>
        /// <param name="fov">Field of view around the cube</param>
        public Cube(double size, double fov = 3.2)
        {
            _size = size * fov;
            _fov = fov;
            _ledgeLength = size;
            _ledgeStep = (int) _ledgeLength;
        }

        /// <summary>
        ///     Rotation of the X axis
        /// </summary>
        public double AngleX
        {
            get => _angleX;
            set => _angleX = value % 360f;
        }

        /// <summary>
        ///     Rotation of the Y axis
        /// </summary>
        public double AngleY
        {
            get => _angleY;
            set => _angleY = value % 360f;
        }

        /// <summary>
        ///     Rotation of the Z axis
        /// </summary>
        public double AngleZ
        {
            get => _angleZ;
            set => _angleZ = value % 360f;
        }

        /// <summary>
        ///     Prints a 2d representation of the cube with the current rotation into a VScreen instance.
        /// </summary>
        /// <param name="screen">VScreen instance to print to</param>
        public void ProjectToVScreen(VScreen screen)
        {
            Parallel.ForEach(Lines, line =>
            {
                Scalar3D diff = line.A - line.B;
                Parallel.For(0, _ledgeStep, i =>
                {
                    // Find a point on the line between A and B.
                    Scalar3D p = line.A + (i / _ledgeLength - 1) * diff;
                    // Moves the point to where it is if the cube is rotated
                    p.Rotate(_angleX, _angleY, _angleZ);
                    // Projects the point into 2d space. The parameters act as a kind of camera setting.
                    Scalar3D q = p.Project(_size, _fov);
                    // Choosing the correct screen coordinates to print at
                    int x = (int) ((q.X + screen.WindowWidth * ViewFactor) / PointDivisor);
                    int y = (int) ((q.Y + screen.WindowHeight * ViewFactor) / PointDivisor);
                    // Pushes the character to the screen
                    screen.Push(CubeChar, x, y);
                });
            });
        }

        /// <summary>
        ///     Contains two 3D points that represent a starting point and an ending point of a line.
        /// </summary>
        private struct LineData
        {
            public readonly Scalar3D A;
            public readonly Scalar3D B;

            public LineData(Scalar3D a, Scalar3D b)
            {
                A = a;
                B = b;
            }
        }
    }
}