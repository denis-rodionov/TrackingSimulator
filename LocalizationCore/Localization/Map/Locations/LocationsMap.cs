using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.BuildingModel;
using LocalizationCore.Primitives;

namespace LocalizationCore.Localization.Locations
{
    [Serializable]
    class LocationsMap : IEnumerable<Location>
    {
        public const double GRID_STEP = 1.5;
        //const double SMALL_LOCATION_PERCENT = 0.4;      // less than 40%

        List<Location> locations = new List<Location>();      
            
        [NonSerialized]
        private Floor floor;

        public LocationsMap()
        {
            floor = Building.Instance.Floor;
        }

        public LocationsMap(Floor floor)
        {
            this.floor = floor;
            CreateLocationMap();
        }

        //private void MergeSmallLocations()
        //{            
        //    while (true)
        //    {
        //        bool noSmallLocations = true;   // assumption

        //        for (int i = 0; i < list.Count(); i++)
        //        {
        //            var loc = list[i];
        //            if (loc.Area < GRID_STEP * GRID_STEP * SMALL_LOCATION_PERCENT)
        //            {
        //                noSmallLocations = false;
        //                Location neighbour = FindBiggerNeighbour();
        //                MergeLocations(loc, neighbour);
        //                break;
        //            }
        //        }

        //        if (noSmallLocations)
        //            break;
        //    }
        //}
        
        private void SplitLocationsByWalls()
        {
            //foreach (var location in list)
            //    CutLocation(location);
        }

        private void CreateLocationMap()
        {
            for (double x = 0; x < floor.Width; x += GRID_STEP)
                for (double y = 0; y < floor.Height; y += GRID_STEP)
                    locations.Add(Location.CreateSquareLocation(new Coord(x, y), GRID_STEP));
        }

        public IEnumerator<Location> GetEnumerator()
        {
            return locations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return locations.GetEnumerator();
        }

        public Location FindLocation(Coord coord)
        {
            Location res = null;

            foreach (var loc in locations)
                if (loc.Include(coord))
                {
                    res = loc;
                    break;
                }

            if (res == null)
                res = null;

            return res;
        }
    }
}
