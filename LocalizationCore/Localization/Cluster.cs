using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Devices;
using LocalizationCore.Localization.FingerprintingAlgorithms;
using LocalizationCore.Localization.Map;

namespace LocalizationCore.Localization
{
    public class Cluster : IEnumerable<Observations>
    {
        public static double SIMILARITY_COEF = 0.22;

        //private Dictionary<Location, Observations> Locations { get; set; }
        RadioMap Locations;     // clustered locations

        public double LocationCount { get { return Locations.Count(); } }

        // Cache
        public Dictionary<Device, double> StandardDeviations { get; private set; }
        public Dictionary<Device, double> Means { get; private set; }
        public IEnumerable<Fingerprint> Measurements { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Cluster()
        {
            Locations = new RadioMap();
        }

        public void Add(Location location, Observations observations)
        {
            Locations.Add(location, observations);
            CacheCalculate();
        }

        public static Cluster Merge(Cluster cluster1, Cluster cluster2)
        {
            var res = new Cluster();
            res.Locations.Add(cluster1.Locations);
            res.Locations.Add(cluster2.Locations);
            res.CacheCalculate();
            return res;
        }

        private void CacheCalculate()
        {
            Measurements = GetMeasurements();
            StandardDeviations = new Dictionary<Device, double>();
            Means = new Dictionary<Device, double>();
            foreach (var dev in Measurements.First().Rss.Keys)
            {
                var rssList = GetRss(dev);
                StandardDeviations.Add(dev, Math.Sqrt(rssList.Variance()));
                Means.Add(dev, rssList.Mean());
            }            
        }

        public static bool IsSimilar(Cluster cl1, Cluster cl2)
        {
            var devices = cl1.Locations.First().MeanRss.Rss.Keys;
            foreach (var dev in devices)
            {
                var coef = Gauss.OverlappingCoefficient(cl1.Means[dev], cl2.Means[dev], 
                            cl1.StandardDeviations[dev], cl2.StandardDeviations[dev]);

                if (coef < SIMILARITY_COEF)
                    return false;
            }

            return true;
        }

        private IEnumerable<double> GetRss(Device dev)
        {
            return Measurements.Select(m => m.Rss[dev]).ToList();
        }

        public IEnumerable<Fingerprint> GetMeasurements()
        {
            var res = new List<Fingerprint>();
            foreach (var l in Locations)
                res.AddRange(l.Fingerprints);
            return res;
        }

        public static bool IsNeighbours(Cluster cl1, Cluster cl2)
        {
            double step = LocationMap.Instance.LocationSize * 1.1;

            foreach (var l1 in cl1.Locations)
                foreach (var l2 in cl2.Locations)
                    if (Coord.Distance(l1.LocationLink.Center, l2.LocationLink.Center) < step)
                        return true;

            return false;
        }

        public IEnumerator<Observations> GetEnumerator()
        {
            return Locations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Locations.GetEnumerator();
        }

        
    }
}
