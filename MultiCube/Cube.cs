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

        const int LEDGE_LENGTH = 25; // Maximum cube ledge length of 25 units. Changing this value will lead to issues.
        private readonly float size, fov;

        // All 
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

        // A LINQ query that puts all valid corner coordinates into an IEnumerable<CornerData> instance.
        private readonly static IEnumerable<CornerData> lines =
            from a in corners
            from b in corners
            where (a - b).Length == 2 && a.X + a.Y + a.Z > b.X + b.Y + b.Z
            select new CornerData(a, b);

        public Cube(float size, float fov = 3)
        {
            this.size = size;
            this.fov = fov;
        }
        public void Update2DProjection(float angleX, float angleY, float angleZ, VScreen screen)
        {
            foreach (CornerData line in lines)
            {
                Point3D diff = line.a - line.b;
                for (int i = 0; i < LEDGE_LENGTH; i++)
                {
                    // Find a point between A and B by following formula p=a+z(b-a) 
                    Point3D p = line.a + ((float)i / LEDGE_LENGTH - 1) * diff;
                    // Rotates the point relative to all the angles given to the method.
                    Point3D r = p.RotateX(angleX).RotateY(angleY).RotateZ(angleZ);
                    // Projects the point into 2d space. The parameters act as a kind of camera setting.
                    Point3D q = r.Project(size, fov);
                    // Setting the cursor to the proper positions
                    int x = ((int)(q.X + screen.WindowWidth * 2.5) / 5);
                    int y = ((int)(q.Y + screen.WindowHeight * 2.5) / 5);

                    screen.Push('°', x, y); // Pushes the character to the screen buffer
                }
            }
        }
    }
}