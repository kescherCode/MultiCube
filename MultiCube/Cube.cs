using System;
using System.Linq;
using System.Collections.Generic;

namespace MultiCube
{
    class Cube
    {
        public class CornerData
        {
            public Point3D a;
            public Point3D b;
            public CornerData(Point3D a, Point3D b)
            {
                this.a = a;
                this.b = b;
            }
        }

        const int LEDGE_LENGTH = 25; // If we draw more characters than that, it starts becoming a grid.
        private readonly float size, fov;

        private readonly static List<Point3D> corners = new List<Point3D>
            {
                new Point3D(-1, -1, -1),
                new Point3D(1, -1, -1),
                new Point3D(1, -1, 1),
                new Point3D(-1, -1, 1),
                new Point3D(-1, 1, 1),
                new Point3D(-1, 1, -1),
                new Point3D(1, 1, -1),
                new Point3D(1, 1, 1)
            };

        // A LINQ query that puts all valid corner coordinates into a simple collection of CornerData instances.
        private readonly static IEnumerable<CornerData> lines =
            from a in corners
            from b in corners
            where (a - b).Length == 2 && a.x + a.y + a.z > b.x + b.y + b.z
            select new CornerData(a, b);

        public Cube(float size, float fov = 3)
        {
            this.size = size;
            this.fov = fov;
        }
        public void Print2DProjection(float angleX, float angleY, float angleZ, VScreen screen)
        {
            foreach (CornerData line in lines)
            {
                for (int i = 0; i < LEDGE_LENGTH; i++)
                {
                    // Find a point between A and B by following formula p=a+z(b-a) where z
                    // is a value between 0 and 1.
                    var point = line.a + (i * 1.0f / 24) * (line.b - line.a);
                    // Rotates the point relative to all the angles given to the method.
                    Point3D r = point.RotateX(angleX).RotateY(angleY).RotateZ(angleZ);
                    // Projects the point into 2d space. Acts as a kind of camera setting.
                    Point3D q = r.Project(size, fov);
                    // Setting the cursor to the proper positions
                    int x = ((int)(q.x + screen.WindowWidth * 2.5) / 5);
                    int y = ((int)(q.y + screen.WindowHeight * 2.5) / 5);

                    screen.Push('°', x, y); // Max Wichmann suggested this symbol
                }
            }
        }
    }
}