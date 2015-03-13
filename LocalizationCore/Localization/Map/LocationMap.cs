using LocalizationCore.BuildingModel;
using LocalizationCore.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.Map
{
    public class LocationMap : IEnumerable<Location>
    {
        const double LOCATION_SIZE = 2;
        const double MAP_OFFSET = -2 * LOCATION_SIZE;

        List<Location> list = new List<Location>();

        public double LocationSize { get; set; }

        private LocationMap()
        {
            LocationSize = LOCATION_SIZE;
        }

        static LocationMap instance = null;
        public static LocationMap Instance
        {
            get
            {
                if (instance == null)
                    CreateMap();
                return instance;
            }
        }

        private static void CreateMap()
        {
            instance = new LocationMap();            

            for (double x = MAP_OFFSET; x <= Building.Instance.Floor.Width; x += instance.LocationSize)
                for (double y = MAP_OFFSET; y <= Building.Instance.Floor.Height; y += instance.LocationSize)
                {
                    var location = new Location(new SRect(new Coord(x, y), 
                                new Coord(x + instance.LocationSize, y + instance.LocationSize)));
                    instance.Add(location);
                }
        }

        public void Add(Location location)
        {
            list.Add(location);
        }

        public IEnumerator<Location> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        /// Find location in the given map
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public Location FindLocation(Coord coord)
        {
            foreach (var l in list)
                if (l.Contains(coord))
                    return l;
            return null;
        }    
    }
}
