using LocalizationCore.Primitives;
using LocalizationCore.SensorModel.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.ErrorEstimation
{
    public class DistanceErrorMapBuilder : BaseErrorMapBuilder
    {
        const double FITTING_COEF = 20;

        RadioMap Map { get; set; }
        IEnumerable<Device> Devices { get; set; }

        public DistanceErrorMapBuilder(RadioMap map, IEnumerable<Device> devices)
        {
            Map = map;
            Devices = devices;
        }

        public override void BuildAccuracyMap()
        {
            foreach (var observations in Map)
            {
                double sumDist = 0;
                foreach (var dev in Devices)
                    sumDist += Coord.Distance(dev.Coordinate, observations.LocationLink.Center);
                var acc = sumDist / FITTING_COEF;
                observations.Accuracy = new Coord(acc, acc);
            }
        }
    }
}
