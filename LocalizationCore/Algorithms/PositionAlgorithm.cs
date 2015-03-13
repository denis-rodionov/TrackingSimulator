using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.PersonModel;
using LocalizationCore.Primitives;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.Algorithms
{
    delegate void NewCoordinateEventHandler(Coord newCoord);

    class PositionAlgorithm
    {
        const double ACCURACY = 1; // for simulation

        MyRandom _random = new MyRandom();
        Coord _lastCoord = null;

        public Person Person { get; set; }
        public event NewCoordinateEventHandler NewCoordinate;

        public PositionAlgorithm(Person person)
        {
            Person = person;
        }

        public Coord LastCoord
        {
            get { return _lastCoord; }
            set
            {
                _lastCoord = value;
                if (NewCoordinate != null)
                    NewCoordinate(_lastCoord);
            }
        }

        public Coord getCoord()
        {
            Coord res = new Coord(-1, -1);
            Floor floor = Building.Instance.Floor;
            Room room = null;
            while (room != floor.findRoom(Person.Coordinate))
            {
                res.X = Person.Coordinate.X + _random.NextDouble(ACCURACY);
                res.Y = Person.Coordinate.Y + _random.NextDouble(ACCURACY);
                room = floor.findRoom(res);
            }

            LastCoord = res;

            return res;
        }
    }
}
