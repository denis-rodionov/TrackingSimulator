using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.Algorithms
{
    class LastCoords
    {
        public TimeSpan BeginTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Coord LastCoord { get; private set; }

        private List<Coord> _coords = new List<Coord>();

        public void addCoord(Coord coord, TimeSpan modelTime)
        {
            if (_coords.Count() == 0)
                BeginTime = modelTime;

            _coords.Add(coord);
            EndTime = modelTime;
            LastCoord = coord;
        }

        public AreaLocation createLocation()
        {
            AreaLocation newLocation = Building.Instance.getLocation(_coords.Last());
            return newLocation;
        }

        public Route createRoute()
        {
            List<AreaLocation> sequence = new List<AreaLocation>();
            AreaLocation lastLoc = null;

            foreach (Coord c in _coords)
            {
                AreaLocation loc = Building.Instance.getLocation(c);
                if (loc != lastLoc)
                {
                    sequence.Add(loc);
                    lastLoc = loc;
                }
            }
            return new Route(sequence);
        }

        public void reset()
        {
            _coords.Clear();
            BeginTime = TimeSpan.Zero;
            EndTime = TimeSpan.Zero;
        }

        public Interval getInterval()
        {
            return new Interval(BeginTime, EndTime);
        }
    }
}
