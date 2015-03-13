using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;

namespace LocalizationCore.BuildingModel
{
    public  enum RoomType { BedRoom, Coridor, Kithen, Toilet, DinnerRoom, Other }

    public class Room
    {
        public SRect Area { private set; get; }
        public string Name { private set; get; }
        public RoomType Type { set; get; }

        static Random _random = new Random();

        public Room(string name, SRect area)
        {
            Type = RoomType.Other;
            Area = area;
            Name = name;
        }

        public override string ToString()
        {
            return "{ Room: " + Name + " }";
        }

        public Coord getRandomPoint()
        {
            double x = _random.NextDouble() * Area.Width + Area.TopLeft.X;
            double y = _random.NextDouble() * Area.Height + Area.TopLeft.Y;
            return new Coord(x, y);
        }
    }
}
