using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;

namespace LocalizationCore.BuildingModel
{
    public class PrivateRoom : Room
    {
        public SRect BedLocation { private set; get; }

        public PrivateRoom(string name, SRect area)
            : base(name, area)
        {
            BedLocation = new SRect(new Coord(area.BottomLeft.X, area.BottomLeft.Y-2), 
                                   new Coord(area.BottomLeft.X + 1, area.BottomLeft.Y));
        }
    }
}
