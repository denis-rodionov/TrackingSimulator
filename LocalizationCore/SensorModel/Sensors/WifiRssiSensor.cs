using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.BuildingModel;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel.Devices;

namespace LocalizationCore.SensorModel.Sensors
{
    public class WifiRssiSensor : RssiSensor
    {
        public const double RSSI_NOISE = 0.3;

        /// <summary>
        /// Rss calculation by Okumura Hata model
        /// </summary> 
        /// <param name="ap"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override double RssiCalculation(AccessPoint ap, Coord coord)
        {
            double dist = Coord.Distance(ap.Coordinate, coord);
            double nu = 1.8;
            double res = OkumuraHata(dist, nu, ap.SignalStrength, RSSI_NOISE);
            return res;
        }

        public override IEnumerable<Device> GetAccessPoints()
        {
            return Building.Instance.Floor.Devices.Where(d => d is WifiAccessPoint).ToList();
        }
        
    }
}
 