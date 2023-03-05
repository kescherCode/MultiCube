using static MultiCube.Globals;

namespace MultiCube
{
    /// <summary>
    ///     A class that stores a cube and allows 2D projection into a VScreen class.
    /// </summary>
    internal class Cube
    {
        // These constants "magnify" the view field and divide the total of a coordinate, screen width and viewFactor.
        private const double ViewFactor = 2.5f, PointDivisor = 5;

        private static readonly LineData[] Lines =
        {
            #region Lines between corners of a cube

            new()
            {
                A = new Scalar3D {X = 1, Y = -1, Z = -1},
                B = new Scalar3D {X = -1, Y = -1, Z = -1}
            },

            new()
            {
                A = new Scalar3D {X = 1, Y = -1, Z = 1},
                B = new Scalar3D {X = 1, Y = -1, Z = -1}
            },

            new()
            {
                A = new Scalar3D {X = 1, Y = -1, Z = 1},
                B = new Scalar3D {X = -1, Y = -1, Z = 1}
            },

            new()
            {
                A = new Scalar3D {X = -1, Y = -1, Z = 1},
                B = new Scalar3D {X = -1, Y = -1, Z = -1}
            },

            new()
            {
                A = new Scalar3D {X = -1, Y = 1, Z = 1},
                B = new Scalar3D {X = -1, Y = -1, Z = 1}
            },

            new()
            {
                A = new Scalar3D {X = -1, Y = 1, Z = 1},
                B = new Scalar3D {X = -1, Y = 1, Z = -1}
            },

            new()
            {
                A = new Scalar3D {X = -1, Y = 1, Z = -1},
                B = new Scalar3D {X = -1, Y = -1, Z = -1}
            },

            new()
            {
                A = new Scalar3D {X = 1, Y = 1, Z = -1},
                B = new Scalar3D {X = 1, Y = -1, Z = -1}
            },

            new()
            {
                A = new Scalar3D {X = 1, Y = 1, Z = -1},
                B = new Scalar3D {X = -1, Y = 1, Z = -1}
            },

            new()
            {
                A = new Scalar3D {X = 1, Y = 1, Z = 1},
                B = new Scalar3D {X = 1, Y = -1, Z = 1}
            },

            new()
            {
                A = new Scalar3D {X = 1, Y = 1, Z = 1},
                B = new Scalar3D {X = -1, Y = 1, Z = 1}
            },

            new()
            {
                A = new Scalar3D {X = 1, Y = 1, Z = 1},
                B = new Scalar3D {X = 1, Y = 1, Z = -1}
            }

            #endregion
        };

        // _ledgeStep is _ledgeLength
        private readonly int _ledgeStep;
        private readonly double _size, _fov, _ledgeLength;

        private double _angleX, _angleY, _angleZ;

        /// <summary>
        ///     Initializes a new Cube instance.
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
            foreach (var line in Lines)
            {
                var diff = line.A - line.B;
                for (var j = 0; j < _ledgeStep; ++j)
                {
                    // Find a point on the line between A and B.
                    var p = line.A + (j / _ledgeLength - 1) * diff;
                    // Moves the point to where it is if the cube is rotated
                    p.Rotate(_angleX, _angleY, _angleZ);
                    // Projects the point into 2d space. The parameters act as a kind of camera setting.
                    var q = p.Project(_size, _fov);
                    // Choosing the correct screen coordinates to print at
                    var x = (int) ((q.X + screen.WindowWidth * ViewFactor) / PointDivisor);
                    var y = (int) ((q.Y + screen.WindowHeight * ViewFactor) / PointDivisor);
                    // Pushes the character to the screen
                    if (screen[x, y] != CubeCharFg)
                        screen.Push(p.Z < 0.3d ? CubeCharFg : CubeCharBg, x, y);
                }
            }
        }

        /// <summary>
        ///     Contains two 3D points that represent a starting point and an ending point of a line.
        /// </summary>
        private struct LineData
        {
            public Scalar3D A;
            public Scalar3D B;
        }
    }
}