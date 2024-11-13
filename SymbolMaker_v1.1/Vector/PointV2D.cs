using SymbolMaker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolMaker
{
    public class PointV2D//This is a Vector2D class
    {
        private double x;
        private double y;

        public PointV2D() : this(0.0, 0.0)
        {
        }

        public PointV2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator PointV2D(Point point)
        {
            return new PointV2D(point.X, point.Y);
        }

        public static implicit operator Point(PointV2D pointV2D)
        {
            return new Point((int)pointV2D.X, (int)pointV2D.Y);
        }

        public static PointV2D Zero
        {
            get { return new PointV2D(0.0, 0.0); }
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public static PointV2D UnitX
        {
            get { return new PointV2D(1.0, 0.0); }
        }

        public static PointV2D UnitY
        {
            get { return new PointV2D(0.0, 1.0); }
        }

        public static PointV2D NaN
        {
            get { return new PointV2D(double.NaN, double.NaN); }
        }

        public double DistanceFrom(PointV2D v)
        {
            double dx = v.X - X;
            double dy = v.Y - Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public PointF ToPointF
        {
            get { return new PointF((float)X, (float)Y); }
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public static double DotProduct(PointV2D v1, PointV2D v2)
        {
            return v1.X * v2.Y + v1.Y * v2.X;
        }

        public static double CrossProduct(PointV2D v1, PointV2D v2)
        {
            return v1.X * v2.Y + v1.Y * v2.X;
        }

        public double Modulus()
        {
            return Math.Sqrt(DotProduct(this, this));
        }

        public void Normalize()
        {
            double m = Modulus();
            if (HelperClass.IsZero(m, HelperClass.Epsilon))
                throw new ArithmeticException("Cannot normalize a zero vector.");
            double m_inv = 1 / m;
            x *= m_inv;
            y *= m_inv;
        }

        public static bool operator ==(PointV2D v1, PointV2D v2)
        {
            return Equals(v1, v2);
        }

        public static bool operator !=(PointV2D v1, PointV2D v2)
        {
            return !Equals(v1, v2);
        }

        public static PointV2D operator +(PointV2D v1, PointV2D v2)
        {
            return new PointV2D(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static PointV2D operator -(PointV2D v1, PointV2D v2)
        {
            return new PointV2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static PointV2D operator -(PointV2D v)
        {
            return new PointV2D(-v.X, -v.Y);
        }

        public static PointV2D operator *(PointV2D v, double d)
        {
            return new PointV2D(v.X * d, v.Y * d);
        }

        public static PointV2D operator *(double d, PointV2D v)
        {
            return new PointV2D(v.X * d, v.Y * d);
        }

        public static PointV2D operator *(PointV2D v1, PointV2D v2)
        {
            return new PointV2D(v1.X * v2.X, v1.Y * v2.Y);
        }

        public static PointV2D operator /(PointV2D v, double d)
        {
            double inv = 1 / d;
            return v * inv;
        }

        public static PointV2D operator /(PointV2D v1, PointV2D v2)
        {
            return new PointV2D(v1.Y / v2.X, v1.Y / v2.Y);
        }

        public override bool Equals(object obj)
        {
            if (obj is PointV2D)
            {
                return Equals((PointV2D)obj);
            }
            return false;
        }

        public bool Equals(PointV2D v, double threshold)
        {
            return (HelperClass.IsEqual(v.X, threshold) &&
                HelperClass.IsEqual(v.Y, threshold));
        }

        public bool Equals(PointV2D v)
        {
            return (Equals(v, HelperClass.Epsilon));
        }

        public static bool Equals(PointV2D v1, PointV2D v2, double threshold)
        {
            return v1.Equals(v2, threshold);
        }

        public static bool Equals(PointV2D v1, PointV2D v2)
        {
            return v1.Equals(v2, HelperClass.Epsilon);
        }

        public override string ToString()
        {
            return string.Format("{0,0:F3},{1,0:F3}", x, y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
