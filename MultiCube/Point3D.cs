using System;

namespace MultiCube
{
    class Point3D
    {
        public float x;
        public float y;
        public float z;

        public Point3D(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public Point3D RotateX(float angle)
        {
            float rad = angle * (float)Math.PI / 180;

            float cosa = (float)Math.Cos(rad);
            float sina = (float)Math.Sin(rad);

            float oldY = y;
            // New Y and Z axis'
            y = y * cosa - z * sina;
            z = oldY * sina + z * cosa;
            return this;
        }

        public Point3D RotateY(float angle)
        {
            float rad = angle * (float)Math.PI / 180;

            float cosa = (float)Math.Cos(rad);
            float sina = (float)Math.Sin(rad);

            float oldX = x;
            // New X and Z axis'
            x = z * sina + x * cosa;
            z = z * cosa - oldX * sina;
            return this;
        }

        public Point3D RotateZ(float angle)
        {
            float rad = angle * (float)Math.PI / 180;

            float cosa = (float)Math.Cos(rad);
            float sina = (float)Math.Sin(rad);

            float oldX = x;
            // New X and Y axis'
            x = x * cosa - y * sina;
            y = y * cosa + oldX * sina;
            return this;
        }

        // Project the current Point into 2D plotted space using X and Y axis'
        public Point3D Project(float width, float height, float projectionSize, float fov)
        {
            float factor = projectionSize / (fov + z);
            Point3D p = new Point3D(x, y, z);
            return new Point3D(p.x * factor + width / 2, -p.y * factor + height / 2, 1);
        }

        // Returns the sum of all coordinates squared.
        public float Length { get { return (float)Math.Sqrt(x * x + y * y + z * z); } }
        public static Point3D operator *(float scale, Point3D x)
        {
            Point3D p = new Point3D(x.x, x.y, x.z);
            p.x *= scale;
            p.y *= scale;
            p.z *= scale;
            return p;
        }

        public static Point3D operator -(Point3D left, Point3D right)
        {
            Point3D p = new Point3D(left.x, left.y, left.z);
            p.x -= right.x;
            p.y -= right.y;
            p.z -= right.z;
            return p;
        }

        public static Point3D operator +(Point3D left, Point3D right)
        {
            Point3D p = new Point3D(left.x, left.y, left.z);
            p.x += right.x;
            p.y += right.y;
            p.z += right.z;
            return p;
        }
    }
}
