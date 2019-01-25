using System;
using System.Diagnostics.CodeAnalysis;

namespace MultiCube
{
    /// <summary>
    ///     Stores 3D coordinates.
    /// </summary>
    internal struct Scalar3D
    {
        public double X;
        public double Y;
        public double Z;

        /// <summary>
        ///     Sets the values in a new Scalar3D struct.
        ///     For internal use.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        private Scalar3D(double x = 0d, double y = 0d, double z = 0d)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public void Rotate(double x = 0d, double y = 0d, double z = 0d)
        {
            if (x != 0d)
            {
                // X
                double rad = x * Math.PI / 180d;

                double cosa = Math.Cos(rad);
                double sina = Math.Sin(rad);

                // Old Y coordinate
                double old = Y;
                // New Y and Z coordinates'
                Y = Y * cosa - Z * sina;
                Z = old * sina + Z * cosa;
            }

            if (y != 0d)
            {
                // Y
                double rad = y * Math.PI / 180d;

                double cosa = Math.Cos(rad);
                double sina = Math.Sin(rad);

                // Old X coordinate
                double old = X;
                // New X and Z coordinates'
                X = Z * sina + X * cosa;
                Z = Z * cosa - old * sina;
            }

            if (z == 0d) return;
            {
                // Z
                double rad = z * Math.PI / 180d;

                double cosa = Math.Cos(rad);
                double sina = Math.Sin(rad);

                // Old X coordinate
                double old = X;
                // New X and Y axis'
                X = X * cosa - Y * sina;
                Y = Y * cosa + old * sina;
            }
        }

        // ReSharper disable UnusedMember.Global
        public Scalar3D RotateX(double angle)
        {
            double radius = angle * Math.PI / 180d;

            double cosa = Math.Cos(radius);
            double sina = Math.Sin(radius);

            double oldY = Y;
            // New Y and Z coordinates'
            double newY = Y * cosa - Z * sina;
            double newZ = oldY * sina + Z * cosa;
            return new Scalar3D(X, newY, newZ);
        }

        public Scalar3D RotateY(double angle)
        {
            double radius = angle * Math.PI / 180d;

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
            double radius = angle * Math.PI / 180d;

            double cosa = Math.Cos(radius);
            double sina = Math.Sin(radius);

            double oldX = X;
            // New X and Y axis'
            X = X * cosa - Y * sina;
            Y = Y * cosa + oldX * sina;
            return this;
        }
        // ReSharper restore UnusedMember.Global

        // Project the current Point into 2D plotted space using X and Y axis'
        public Scalar3D Project(double projectionSize, double fov)
        {
            double factor = projectionSize / (fov + Z);
            return new Scalar3D(X * factor, -Y * factor);
        }

        // Returns the sum of all coordinates squared.
        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        public static Scalar3D operator *(double scale, Scalar3D x)
        {
            x.X *= scale;
            x.Y *= scale;
            x.Z *= scale;
            return x;
        }

        public static Scalar3D operator /(double scale, Scalar3D x)
        {
            x.X /= scale;
            x.Y /= scale;
            x.Z /= scale;
            return x;
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

        // ReSharper disable once MemberCanBePrivate.Global
        public bool Equals(Scalar3D scalar3D)
        {
            Scalar3D point = scalar3D;
            return X.Equals(point.X) && Y.Equals(point.Y) && Z.Equals(point.Z);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return new {X, Y, Z}.GetHashCode();
        }
    }
}