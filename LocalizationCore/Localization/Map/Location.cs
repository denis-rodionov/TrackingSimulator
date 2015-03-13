using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogProvider;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Devices;
using System.Collections.ObjectModel;
using LocalizationCore.Painting;
using LocalizationCore.Localization.Locations;

namespace LocalizationCore.Localization
{
    [Serializable]
    public class Location
    {
        #region Properties
        
        public Coord Center { get { return Polygone.Center; }  }

        public SPolygone Polygone { get; private set; }
        
        #endregion

        #region init

        public Location(SRect rect)
        {
            Polygone = new SPolygone(rect);
        }

        public static Location CreateSquareLocation(Coord topLeft, double sideLength)
        {
            var rect = new SRect(topLeft, new Coord(topLeft.X + sideLength, topLeft.Y + sideLength));
            var location = new Location(rect);

            return location;
        }

        #endregion

        #region public

        public bool Contains(Coord coord)
        {
            return Polygone.ContainsInsideRect(coord);
        }

        public override string ToString()
        {
            return "{" + Center.X + ", " + Center.Y + "}";
        }        

        /// <summary>
        /// Determines, if the point is inside the location
        /// </summary>
        /// <param name="coord">The point</param>
        /// <returns></returns>
        public bool Include(Coord coord)
        {
            double delta = LocationsMap.GRID_STEP;
            if (coord.X + delta < Center.X || coord.X - delta > Center.X ||
                coord.Y + delta < Center.Y || coord.Y - delta > Center.Y)
                return false;

            double lastValue = 0;
            for (int i = 0; i < Polygone.Vertexes.Count() - 1; i++)
            {
                var edge = new SVector(Polygone.Vertexes[i], Polygone.Vertexes[i + 1]);
                var point = new SVector(Polygone.Vertexes[i], coord);

                double crossProduct = edge.CrossProduct(point);

                if (i != 0 && lastValue * crossProduct < 0)
                    return false;
                lastValue = crossProduct;
            }

            return true;
        }

        #endregion
        
        
    }
}
