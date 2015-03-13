using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.BuildingModel;
using LocalizationCore.PersonModel;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel.Devices;

namespace LocalizationCore.SensorModel.Sensors
{
    public abstract class RssiSensor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="carrier"></param>
        public RssiSensor()
        {
        }

        /// <summary>
        /// Measure RSS
        /// </summary>
        /// <returns>List of RSS from every visible point</returns>
        public Fingerprint GetMeasurements(Coord coord)
        {
            var ap = GetAccessPoints();

            var res = new Fingerprint();
            foreach (AccessPoint p in ap)
                res.Rss.Add((Device)p, RssiCalculation(p, coord));

            return res;
        }

        public abstract double RssiCalculation(AccessPoint ap, Coord coord);
        public abstract IEnumerable<Device> GetAccessPoints();

        /// <summary>
        /// Common model to calculate RSSI
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="nu"></param>
        /// <param name="rssi0"></param>
        /// <param name="noiseVariation"></param>
        /// <returns></returns>
        protected double OkumuraHata(double dist, double nu, double rssi0, double noiseVariation)
        {
            return rssi0 - 10 * nu * Math.Log10(dist + FITTING) +SimpleRNG.GetNormal(0, noiseVariation);
        }

        const double FITTING = 0.1;
    }
}
