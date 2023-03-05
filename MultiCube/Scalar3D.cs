﻿using System;
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
                var rad = x * Math.PI / 180d;

                var cosA = Math.Cos(rad);
                var sinA = Math.Sin(rad);

                // Old Y coordinate
                var old = Y;
                // New Y and Z coordinates'
                Y = Y * cosA - Z * sinA;
                Z = old * sinA + Z * cosA;
            }

            if (y != 0d)
            {
                // Y
                var rad = y * Math.PI / 180d;

                var cosA = Math.Cos(rad);
                var sinA = Math.Sin(rad);

                // Old X coordinate
                var old = X;
                // New X and Z coordinates'
                X = Z * sinA + X * cosA;
                Z = Z * cosA - old * sinA;
            }

            if (z == 0d) return;
            {
                // Z
                var rad = z * Math.PI / 180d;

                var cosA = Math.Cos(rad);
                var sinA = Math.Sin(rad);

                // Old X coordinate
                var old = X;
                // New X and Y axis'
                X = X * cosA - Y * sinA;
                Y = Y * cosA + old * sinA;
            }
        }

        // ReSharper disable UnusedMember.Global
        public Scalar3D RotateX(double angle)
        {
            var radius = angle * Math.PI / 180d;

            var cosA = Math.Cos(radius);
            var sinA = Math.Sin(radius);

            var oldY = Y;
            // New Y and Z coordinates'
            var newY = Y * cosA - Z * sinA;
            var newZ = oldY * sinA + Z * cosA;
            return new Scalar3D(X, newY, newZ);
        }

        public Scalar3D RotateY(double angle)
        {
            var radius = angle * Math.PI / 180d;

            var cosA = Math.Cos(radius);
            var sinA = Math.Sin(radius);

            var oldX = X;
            // New X and Z coordinates'
            X = Z * sinA + X * cosA;
            Z = Z * cosA - oldX * sinA;
            return this;
        }

        public Scalar3D RotateZ(double angle)
        {
            var radius = angle * Math.PI / 180d;

            var cosA = Math.Cos(radius);
            var sinA = Math.Sin(radius);

            var oldX = X;
            // New X and Y axis'
            X = X * cosA - Y * sinA;
            Y = Y * cosA + oldX * sinA;
            return this;
        }
        // ReSharper restore UnusedMember.Global

        // Project the current Point into 2D plotted space using X and Y axis'
        public Scalar3D Project(double projectionSize, double fov)
        {
            var factor = projectionSize / (fov + Z);
            return new Scalar3D(X * factor, -Y * factor);
        }

        // Returns the sum of all coordinates squared.
        // ReSharper disable once UnusedMember.Global
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

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var point = (Scalar3D) obj;
            return X.Equals(point.X) && Y.Equals(point.Y) && Z.Equals(point.Z);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool Equals(Scalar3D point)
        {
            return X.Equals(point.X) && Y.Equals(point.Y) && Z.Equals(point.Z);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return new {X, Y, Z}.GetHashCode();
        }
    }
}