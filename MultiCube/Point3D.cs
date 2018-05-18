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
            float radius = angle * (float)Math.PI / 180;

            float cosa = (float)Math.Cos(radius);
            float sina = (float)Math.Sin(radius);

            float oldY = y;
            // New Y and Z coordinates'
            y = y * cosa - z * sina;
            z = oldY * sina + z * cosa;
            return this;
        }

        public Point3D RotateY(float angle)
        {
            float radius = angle * (float)Math.PI / 180;

            float cosa = (float)Math.Cos(radius);
            float sina = (float)Math.Sin(radius);

            float oldX = x;
            // New X and Z coordinates'
            x = z * sina + x * cosa;
            z = z * cosa - oldX * sina;
            return this;
        }

        public Point3D RotateZ(float angle)
        {
            float radius = angle * (float)Math.PI / 180;

            float cosa = (float)Math.Cos(radius);
            float sina = (float)Math.Sin(radius);

            float oldX = x;
            // New X and Y axis'
            x = x * cosa - y * sina;
            y = y * cosa + oldX * sina;
            return this;
        }

        // Project the current Point into 2D plotted space using X and Y axis'
        public Point3D Project(float projectionSize, float fov)
        {
            float factor = projectionSize / (fov + z);
            return new Point3D(x * factor, -y * factor, 1);
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

        public static bool operator ==(Point3D left, Point3D right) => left.x == right.x &&
                                                                       left.y == right.y &&
                                                                       left.z == right.z;
        public static bool operator !=(Point3D left, Point3D right) => !(left == right);
        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Point3D point = (Point3D)obj;
            return (x == point.x && y == point.y && z == point.z);
        }
        public override int GetHashCode() => new { x, y, z }.GetHashCode();
    }
}
