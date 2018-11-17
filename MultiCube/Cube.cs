using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MultiCube.Globals;

namespace MultiCube
{
    // A class for storing a cube and sending a two-dimensional projection to a VScreen instance.
    internal class Cube
    {
        // These constants "magnify" the view field and divide the total of coordinate, screen width and viewFactor.
        private const double ViewFactor = 2.5f, PointDivisor = 5;

        // All possible corners on a cube
        private static readonly List<Scalar3D> AllCorners = new List<Scalar3D>
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
        private static readonly IEnumerable<LineData> Lines = (from a in AllCorners
            from b in AllCorners
            where (a - b).Length == 2 && a.X + a.Y + a.Z > b.X + b.Y + b.Z
            select new LineData(a, b)).ToList();

        private readonly double _ledgeLength, _size, _fov;

        // How many points to draw on the line in-between two points
        private readonly int _ledgeSteps;

        private double _angleX, _angleY, _angleZ;

        /// <summary>
        ///     Initializes a new Cube instanze.
        /// </summary>
        /// <param name="size">Cube size</param>
        /// <param name="fov">Camera field of view</param>
        public Cube(double size, double fov = 3)
        {
            _size = size;
            _fov = fov;
            _ledgeLength = size / ZoomFactor - 1;
            _ledgeSteps = (int) (size / ZoomFactor);
        }

        // "Camera position" for the cubes
        public double AngleX
        {
            get => _angleX;
            set => _angleX = value % 360f;
        }

        public double AngleY
        {
            get => _angleY;
            set => _angleY = value % 360f;
        }

        public double AngleZ
        {
            get => _angleZ;
            set => _angleZ = value % 360f;
        }

        public void UpdateProjection(VScreen screen)
        {
            Parallel.ForEach(Lines, line =>
            {
                // Vector connecting the two points on a line
                Scalar3D diff = line.A - line.B;
                // Drawing points on the line between A and B so user sees a line between the corners in the end
                Parallel.For(0, _ledgeSteps, i =>
                {
                    // Find a point between A and B
                    Scalar3D p = line.A + i / _ledgeLength * diff;
                    // Rotate the point
                    Scalar3D r = p.RotateX(AngleX).RotateY(AngleY).RotateZ(AngleZ);
                    // Project the point into 2d space. The parameters act as a kind of "camera setting".
                    Scalar3D q = r.Project(_size, _fov);
                    // Choosing the screen coordinates to print the intermediate point of the line at
                    int x = (int) ((q.X + screen.WindowWidth * ViewFactor) / PointDivisor);
                    int y = (int) ((q.Y + screen.WindowHeight * ViewFactor) / PointDivisor);
                    // Pushes the character to the screen buffer
                    screen.Push(CubeChar, x, y);
                });
            });
        }

        // Stores a vector
        public struct LineData
        {
            public Scalar3D A;
            public Scalar3D B;

            public LineData(Scalar3D a, Scalar3D b)
            {
                A = a;
                B = b;
            }
        }
    }
}