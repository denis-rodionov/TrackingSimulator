using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;

namespace LocalizationCore.Algorithms
{
    class Route : ChainItem
    {
        List<AreaLocation> _locations = new List<AreaLocation>();

        public Route(List<AreaLocation> locations)
        {
            _locations = locations;
        }

        public static bool operator ==(Route route1, Route route2)
        {
            if ((object)route1 == null || (object)route2 == null)
                return PointerComparer.comparePointers(route1, route2);

            if (route1._locations.Count() != route2._locations.Count())
                return false;

            for (int i = 0; i < route1._locations.Count(); i++)
                if (route1._locations[i] != route2._locations[i])
                    return false;

            return true;       
        }

        public static bool operator !=(Route route1, Route route2)
        {
            return !(route1 == route2);
        }

        public override string ToString()
        {
            string res = "";
            foreach (AreaLocation l in _locations)
                res += l + " -> ";
            res += "||";
            return "{" + res + "}";
        }

        public IEnumerable<AreaLocation> getRouteLocations()
        {
            return _locations;
        }
    }
}
