using System;
using System.Diagnostics.CodeAnalysis;

namespace MultiCube
{
    internal struct Scalar3D
    {
        public double X;
        public double Y;
        public double Z;

        public Scalar3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Scalar3D RotateX(double angle)
        {
            double radius = angle * Math.PI / 180;

            double cosa = Math.Cos(radius);
            double sina = Math.Sin(radius);

            double oldY = Y;
            // New Y and Z coordinates'
            Y = Y * cosa - Z * sina;
            Z = oldY * sina + Z * cosa;
            return this;
        }

        public Scalar3D RotateY(double angle)
        {
            double radius = angle * Math.PI / 180;

            double cosa = Math.Cos(radius);
            double sina = Math.Sin(radius);

            double oldX = X;
            // New X and Z coordinates'
            X = Z * sina + X * cosa;
            Z = Z * cosa - oldX * sina;
            return this;
        }

        public Scalar3D RotateZ(double angle)
        {
            double radius = angle * Math.PI / 180;

            double cosa = Math.Cos(radius);
            double sina = Math.Sin(radius);

            double oldX = X;
            // New X and Y axis'
            X = X * cosa - Y * sina;
            Y = Y * cosa + oldX * sina;
            return this;
        }

        // Project the current Point into 2D plotted space using X and Y axis'
        public Scalar3D Project(double projectionSize, double fov)
        {
            double factor = projectionSize / (fov + Z);
            return new Scalar3D(X * factor, -Y * factor, 1);
        }

        // Returns the sum of all coordinates squared.
        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        public static Scalar3D operator *(double scale, Scalar3D x)
        {
            var p = new Scalar3D(x.X, x.Y, x.Z);
            p.X *= scale;
            p.Y *= scale;
            p.Z *= scale;
            return p;
        }

        public static Scalar3D operator -(Scalar3D left, Scalar3D right)
        {
            var p = new Scalar3D(left.X, left.Y, left.Z);
            p.X -= right.X;
            p.Y -= right.Y;
            p.Z -= right.Z;
            return p;
        }

        public static Scalar3D operator +(Scalar3D left, Scalar3D right)
        {
            var p = new Scalar3D(left.X, left.Y, left.Z);
            p.X += right.X;
            p.Y += right.Y;
            p.Z += right.Z;
            return p;
        }

        public static bool operator ==(Scalar3D left, Scalar3D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Scalar3D left, Scalar3D right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var point = (Scalar3D) obj;
            return X.Equals(point.X) && Y.Equals(point.Y) && Z.Equals(point.Z);
        }

        public bool Equals(Scalar3D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
    }
}