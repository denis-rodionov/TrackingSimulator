using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.Primitives
{
    public class SVector
    {
        Coord _value = new Coord(0, 0);

        public SVector(Coord begin, Coord end)
        {
            _value.X = end.X - begin.X;
            _value.Y = end.Y - begin.Y;
        }

        public SVector(double x, double y)
        {
            _value = new Coord(x, y);
        }

        public SVector(SVector v)
        {
            _value = new Coord(v.X, v.Y);
        }

        public double X
        {
            get { return _value.X; }
            set { _value.X = value; }
        }
        
        public double Y
        {
            get { return _value.Y; }
            set { _value.Y = value; }
        }

        public static SVector operator*(SVector v, double k)
        {
            SVector res = new SVector(v);
            res.X = v.X * k;
            res.Y = v.Y * k;
            return res;
        }

        public static SVector operator /(SVector v, double k)
        {
            return v * (1 / k);
        }

        public static Coord operator +(Coord c, SVector v)
        {
            Coord res = new Coord(c.X, c.Y);
            res.X += v.X;
            res.Y += v.Y;
            return res;
        }

        public static SVector operator +(SVector v1, SVector v2)
        {
            return new SVector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public SVector Normalize()
        {
            double sqSum = Math.Sqrt( X * X + Y * Y );
            X = X / sqSum;
            Y = Y / sqSum;
            return this;
        }

        public SVector Normalize(double length)
        {
            double sqSum = Math.Sqrt(X * X + Y * Y);
            X = X * length / sqSum;
            Y = Y * length / sqSum;
            return this;
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(_value.X * _value.X + _value.Y * _value.Y);
            }
        }

        public double CrossProduct(SVector v)
        {
            return X * v.Y - Y * v.X;
        }

        public static SVector RandomVector(double meanLength, double sigma)
        {
            var alpha = RandomAngle();
            var length = random.NextDouble() * SimpleRNG.GetNormal(meanLength, sigma);

            var x = Math.Cos(alpha) * length;
            var y = Math.Sin(alpha) * length;

            return new SVector(x, y);
        }

        static Random random = new Random();
        private static double RandomAngle()
        {
            return random.NextDouble() * 2 * Math.PI;
        }

        public static Coord Intersection(Coord a, Coord b, Coord c, Coord d)
        {
            var x = ((a.X * b.Y - a.Y * b.X) * (c.X - d.X) - (a.X - b.X) * (c.X * d.Y - c.Y * d.X)) / ((a.X - b.X) * (c.Y - d.Y) - (a.Y - b.Y) * (c.X - d.X));
            var y = ((a.X * b.Y - a.Y * b.X) * (c.Y - d.Y) - (a.Y - b.Y) * (c.X * d.Y - c.Y * d.X)) / ((a.X - b.X) * (c.Y - d.Y) - (a.Y - b.Y) * (c.X - d.X));

            var diff = 0.01 * (b - a);

            return new Coord(x, y) + diff;
        }

        public static bool IsIntersection(Coord a, Coord b, Coord c, Coord d)
        {
            double c1 = CrossProduct(new SVector(a, c), new SVector(b, c));
            double d1 = CrossProduct(new SVector(a, d), new SVector(b, d));
            double a2 = CrossProduct(new SVector(c, a), new SVector(d, a));
            double b2 = CrossProduct(new SVector(c, b), new SVector(d, b));

            return c1 * d1 <= 0 && a2 * b2 <= 0;
        }

        private static double CrossProduct(SVector v1, SVector v2)
        {
            return v1.X * v2.Y - v2.X * v1.Y;
        }
    }
}
