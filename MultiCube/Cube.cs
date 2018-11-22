using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MultiCube.Globals;

namespace MultiCube
{
    // A class for storing a cube and sending a two-dimensional projection to a VScreen instance.
    class Cube
    {
        // Stores a vector
        public struct CornerData
        {
            public Scalar3D a;
            public Scalar3D b;
            public CornerData(Scalar3D a, Scalar3D b)
            {
                this.a = a;
                this.b = b;
            }
        }

        // These constants "magnify" the view field and divide the total of coord, screen width and viewFactor.
        const double viewFactor = 2.5f, pointDivisor = 5;

        readonly int ledgeLength;
        readonly double size, fov;

        // All possible corners on a cube
        readonly static List<Scalar3D> corners = new List<Scalar3D>
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

        // A LINQ query that puts all valid corner coordinates into a collection of CornerData
        readonly static IEnumerable<CornerData> lines = (from a in corners
                                                        from b in corners
                                                        where (a - b).Length == 2 && a.X + a.Y + a.Z > b.X + b.Y + b.Z
                                                        select new CornerData(a, b)).ToList();

        private double angleX = 0f, angleY = 0f, angleZ = 0f;
        // "Camera position" for the cubes
        public double AngleX
        {
            get => angleX;
            set => angleX = value % 360f;
        }
        public double AngleY
        {
            get => angleY;
            set => angleY = value % 360f;
        }
        public double AngleZ
        {
            get => angleZ;
            set => angleZ = value % 360f;
        }

        /// <summary>
        /// Initializes a new Cube instanze.
        /// </summary>
        /// <param name="size">Cube size</param>
        /// <param name="fov"></param>
        public Cube(double size, double fov = 3)
        {
            this.size = size;
            this.fov = fov;
            ledgeLength = (int)(size / ZOOM_FACTOR);
        }
        public void UpdateProjection(VScreen screen)
        {
            Parallel.ForEach(lines, line =>
            {
                Scalar3D diff = line.a - line.b;
                Parallel.For(0, ledgeLength, i =>
                {
                    // Find a point between A and B
                    Scalar3D p = line.a + ((double)i / ledgeLength - 1) * diff;
                    // Rotates the point
                    Scalar3D r = p.RotateX(AngleX).RotateY(AngleY).RotateZ(AngleZ);
                    // Projects the point into 2d space. The parameters act as a kind of camera setting.
                    Scalar3D q = r.Project(size, fov);
                    // Choosing the screen coordinates to print at
                    int x = (int)((q.X + screen.WindowWidth * viewFactor) / pointDivisor);
                    int y = (int)((q.Y + screen.WindowHeight * viewFactor) / pointDivisor);
                    // Pushes the character to the screen buffer
                    screen.Push(CUBE_CHAR, x, y);
                });
            });
        }
    }
}