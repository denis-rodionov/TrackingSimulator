using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;

namespace LocalizationCore.BuildingModel
{
    public class Door
    {
        public Coord Begin { private set; get; }
        public Coord End { private set; get; }

        public Door(Coord begin, Coord end)
        {
            Begin = begin;
            End = end;
        }

        public Coord getCenter()
        {
            double x = (Begin.X + End.X) / 2;
            double y = (Begin.Y + End.Y) / 2;
            return new Coord(x, y);
        }
    }
}
