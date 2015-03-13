using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.Primitives
{
    [Serializable]
    public class Coord
    {
        const double COMPARE_ERROR = 1e-3;

        public double X { get; set; }
        public double Y { get; set; }

        public Coord(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "(" + X.ToString("F") + ", " + Y.ToString("F") + ")";
        }

        public Coord clone()
        {
            return new Coord(X, Y);
        }

        public static double Distance(Coord coord1, Coord coord2)
        {
            double x = coord2.X - coord1.X;
            double y = coord2.Y - coord1.Y;
            return Math.Sqrt(x * x + y * y);
        }

        public static bool operator ==(Coord coord1, Coord coord2)
        {
            if ((object)coord1 == null || (object)coord2 == null)
                return PointerComparer.comparePointers(coord1, coord2);
            return Distance(coord1, coord2) < COMPARE_ERROR;
        }

        public static bool operator !=(Coord coord1, Coord coord2)
        {
            return !(coord1 == coord2);
        }

        public static Coord operator +(Coord coord1, Coord coord2)
        {
            return new Coord(coord1.X + coord2.X, coord1.Y + coord2.Y);
        }

        public static Coord operator -(Coord coord1, Coord coord2)
        {
            return new Coord(coord1.X - coord2.X, coord1.Y - coord2.Y);
        }

        public static Coord operator *(double k, Coord coord)
        {
            return new Coord(coord.X * k, coord.Y * k);
        }

        public static Coord operator /(Coord coord, double k)
        {
            return new Coord(coord.X / k, coord.Y / k);
        }
    }
}
