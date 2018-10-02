using System;

namespace MultiCube
{
    class Point3D
    {
        public float X;
        public float Y;
        public float Z;

        public Point3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D RotateX(float angle)
        {
            float radius = angle * (float)Math.PI / 180;

            float cosa = (float)Math.Cos(radius);
            float sina = (float)Math.Sin(radius);

            float oldY = Y;
            // New Y and Z coordinates'
            Y = Y * cosa - Z * sina;
            Z = oldY * sina + Z * cosa;
            return this;
        }

        public Point3D RotateY(float angle)
        {
            float radius = angle * (float)Math.PI / 180;

            float cosa = (float)Math.Cos(radius);
            float sina = (float)Math.Sin(radius);

            float oldX = X;
            // New X and Z coordinates'
            X = Z * sina + X * cosa;
            Z = Z * cosa - oldX * sina;
            return this;
        }

        public Point3D RotateZ(float angle)
        {
            float radius = angle * (float)Math.PI / 180;

            float cosa = (float)Math.Cos(radius);
            float sina = (float)Math.Sin(radius);

            float oldX = X;
            // New X and Y axis'
            X = X * cosa - Y * sina;
            Y = Y * cosa + oldX * sina;
            return this;
        }

        // Project the current Point into 2D plotted space using X and Y axis'
        public Point3D Project(float projectionSize, float fov)
        {
            float factor = projectionSize / (fov + Z);
            return new Point3D(X * factor, -Y * factor, 1);
        }

        // Returns the sum of all coordinates squared.
        public float Length { get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); } }
        public static Point3D operator *(float scale, Point3D x)
        {
            Point3D p = new Point3D(x.X, x.Y, x.Z);
            p.X *= scale;
            p.Y *= scale;
            p.Z *= scale;
            return p;
        }

        public static Point3D operator -(Point3D left, Point3D right)
        {
            Point3D p = new Point3D(left.X, left.Y, left.Z);
            p.X -= right.X;
            p.Y -= right.Y;
            p.Z -= right.Z;
            return p;
        }

        public static Point3D operator +(Point3D left, Point3D right)
        {
            Point3D p = new Point3D(left.X, left.Y, left.Z);
            p.X += right.X;
            p.Y += right.Y;
            p.Z += right.Z;
            return p;
        }

        public static bool operator ==(Point3D left, Point3D right) => left.Equals(right);
        public static bool operator !=(Point3D left, Point3D right) => !left.Equals(right);
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Point3D point = (Point3D)obj;
            return (X == point.X && Y == point.Y && Z == point.Z);
        }
        public override int GetHashCode() => new { X, Y, Z }.GetHashCode();
    }
}
