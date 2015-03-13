using LocalizationCore.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.BuildingModel
{
    public class RoomWindow
    {
        public Coord Start { get; set; }
        public Coord End { get; set; }

        public RoomWindow(Coord start, Coord end)
        {
            Start = start;
            End = end;
        }
    }
}
