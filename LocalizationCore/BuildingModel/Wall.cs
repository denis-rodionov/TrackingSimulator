using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;

namespace LocalizationCore.BuildingModel
{
    public class Wall : ILine
    {
        public Door Door { get; set; }
        public Coord Begin { get; set; }
        public Coord End { get; set; }

        public Wall(Coord begin, Coord end, bool door)
        {
            Begin = begin;
            End = end;
            if (door)
                createDoor();
        }

        private void createDoor()
        {
            SVector v = new SVector(this.Begin, this.End);
            Coord begin = this.Begin + v / 3;
            Coord end = begin + v.Normalize();
            Door = new Door(begin, end);
        }

        public Coord A
        {
            get { return Begin; }
        }

        public Coord B
        {
            get { return End; }
        }

        public override string ToString()
        {
            return A + " - " + B;
        }
    }
}
