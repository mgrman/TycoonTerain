using System;
using System.Globalization;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct Vector3i : IEquatable<Vector3i>
    {
        [Newtonsoft.Json.JsonIgnore]
        public static readonly Vector3i Zero = new Vector3i();

        [Newtonsoft.Json.JsonIgnore]
        public static readonly Vector3i One = new Vector3i(1, 1, 1);

        public readonly int X;

        public readonly int Y;

        public readonly int Z;

        public Vector3i(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        [Newtonsoft.Json.JsonIgnore]
        public Vector2i XY => new Vector2i(X, Y);

        [Newtonsoft.Json.JsonIgnore]
        public bool AllPositive => this.X > 0 && this.Y > 0 && this.Z > 0;

        [Newtonsoft.Json.JsonIgnore]
        public bool AllZeroOrPositive => this.X >= 0 && this.Y >= 0 && this.Z >= 0;

        [Newtonsoft.Json.JsonIgnore]
        public bool AnyNegative => this.X < 0 || this.Y < 0 || this.Z < 0;

        [Newtonsoft.Json.JsonIgnore]
        public bool AnyZero => this.X == 0 || this.Y == 0 || this.Z == 0;

        [Newtonsoft.Json.JsonIgnore]
        public bool AnyZeroOrNegative => this.X <= 0 || this.Y <= 0 || this.Z <= 0;

        [Newtonsoft.Json.JsonIgnore]
        public int Volume => X * Y * Z;

        public static Vector3i operator +(Vector3i a, int b)
        {
            return new Vector3i(a.X + b, a.Y + b, a.Z + b);
        }

        public static Vector3i operator -(Vector3i a, int b)
        {
            return new Vector3i(a.X - b, a.Y - b, a.Z - b);
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3f operator +(Vector3i a, Vector3f b)
        {
            return new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3f operator *(Vector3f a, Vector3i b)
        {
            return new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3f operator *(Vector3i a, Vector3f b)
        {
            return new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3i operator *(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3i operator /(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Vector3f operator /(Vector3i a, Vector3f b)
        {
            return new Vector3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Vector3i operator /(Vector3i a, int b)
        {
            return new Vector3i(a.X / b, a.Y / b, a.Z / b);
        }

        public static Vector3f operator /(Vector3i a, float b)
        {
            return new Vector3f(a.X / b, a.Y / b, a.Z / b);
        }

        public static Vector3i Max(Vector3i a, Vector3i b)
        {
            return new Vector3i(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }

        public static Vector3i Min(Vector3i a, Vector3i b)
        {
            return new Vector3i(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        public static Vector3i FromSame(int value)
        {
            return new Vector3i(value, value, value);
        }

        public static Vector3i Cross(Vector3i lhs, Vector3i rhs)
        {
            return new Vector3i(lhs.Y * rhs.Z - lhs.Z * rhs.Y, lhs.Z * rhs.X - lhs.X * rhs.Z, lhs.X * rhs.Y - lhs.Y * rhs.X);
        }

        public static bool operator <(Vector3i a, Vector3i b)
        {
            return a.X < b.X && a.Y < b.Y && a.Z < b.Z;
        }

        public static bool operator <=(Vector3i a, Vector3i b)
        {
            return a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
        }

        public static bool operator >(Vector3i a, Vector3i b)
        {
            return a.X > b.X && a.Y > b.Y && a.Z > b.Z;
        }

        public static bool operator >=(Vector3i a, Vector3i b)
        {
            return a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;
        }

        public static bool operator ==(Vector3i a, Vector3i b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector3i a, Vector3i b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public void ForeachPointExlusive(Action<Vector3i> action)
        {
            for (int ix = 0; ix < this.X; ix++)
            {
                for (int iy = 0; iy < this.Y; iy++)
                {
                    for (int iz = 0; iz < this.Z; iz++)
                    {
                        action(new Vector3i(ix, iy, iz));
                    }
                }
            }
        }

        public Vector3i DivideUp(Vector3i a, int b)
        {
            return new Vector3i(a.X.DivideUp(b), a.Y.DivideUp(b), a.Z.DivideUp(b));
        }

        public Vector3f ToVector3f()
        {
            return new Vector3f(X, Y, Z);
        }

        public Range3i ToRange3i()
        {
            return Range3i.FromMinAndSize(Vector3i.Zero, this);
        }

        public bool Equals(Vector3i other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3i))
                return false;

            return this.Equals((Vector3i)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X + Y * 7 + Z * 13;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", X, Y, Z);
        }
    }
}