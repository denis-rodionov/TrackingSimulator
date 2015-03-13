using LocalizationCore.Localization.FingerprintingAlgorithms.Rssi;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Devices;
using LocalizationCore.SensorModel.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.FingerprintingAlgorithms
{
    public class HistogramFingerprinting : Fingerprinting
    {
        #region public

        Dictionary<Observations, Dictionary<Device, RssiDistribution>> Distribution { get; set; }

        int BinCount { get; set; }

        public HistogramFingerprinting(string name, RssiSensor sensor, int binCount)
            : base(name, sensor, CalculationAlgorithm.MeanK)
        {
            BinCount = binCount;
            InitDitribution();      // approximation by histograms
        }        

        public override Pdf GetLikelihood(Fingerprint fingerprint)
        {
            var pdf = new Pdf();
            foreach (var ob in RadioMap)
            {
                var temp = Posterior(Distribution[ob], fingerprint);
                pdf.Set(ob.LocationLink, temp);
            }

            return pdf;
        }        

        #endregion

        #region implementation

        private double Posterior(Dictionary<Device, RssiDistribution> rssiDistribution, Fingerprint fingerprint)
        {
            double res = 0;
            foreach (var dev in fingerprint.Rss.Keys)
            {
                var distr = rssiDistribution[dev];
                res += distr.Multiply(fingerprint.Rss[dev]);
            }
            res = res / fingerprint.Rss.Keys.Count;
            return res;
        }

        private void InitDitribution()
        {
            Distribution = new Dictionary<Observations, Dictionary<Device, RssiDistribution>>();
            double max = 20;    // GetMax();
            double min = GetMin();

            foreach (var l in RadioMap)
            {
                Distribution.Add(l, new Dictionary<Device, RssiDistribution>());
                AproximateDistribution(l, Distribution[l], min, max);
            }
        }

        private double GetMin()
        {
            double res = double.MaxValue;
            foreach (var l in RadioMap)
                foreach (var cp in l.Fingerprints)
                    foreach (double rssi in cp.Rss.Values)
                        if (rssi < res)
                            res = rssi;
            return res;
        }

        private double GetMax()
        {
            double res = double.MinValue;
            foreach (var l in RadioMap)
                foreach (var cp in l.Fingerprints)
                    foreach (double rssi in cp.Rss.Values)
                        if (rssi > res)
                            res = rssi;
            return res;
        }

        private void AproximateDistribution(Observations observations, Dictionary<Device, RssiDistribution> distr,
                                            double minRss, double maxRss)
        {
            var devices = observations.Fingerprints.First().Rss.Keys;

            // initialize classes
            foreach (var dev in devices)
                distr.Add(dev, new RssiDistribution(BinCount, minRss, maxRss));

            // creating distribution
            foreach (var cp in observations.Fingerprints)   // by calibration points
                foreach (var dev in devices)
                    distr[dev].AddPoint(cp.Rss[dev]);

            // normalization
            foreach (var dev in devices)
                distr[dev].Normalize();
        }

        #endregion


    }
}
