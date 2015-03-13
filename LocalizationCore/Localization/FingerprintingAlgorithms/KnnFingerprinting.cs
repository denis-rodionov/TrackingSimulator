using LocalizationCore.Primitives;
using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Devices;
using LocalizationCore.SensorModel.Sensors;
using LogProvider.ProcessInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.FingerprintingAlgorithms
{
    public class KnnFingerprinting : Fingerprinting
    {
        const int K = 10;        // coefficient for KNN

        public KnnFingerprinting(string name, RssiSensor sensor)
            : base(name, sensor, CalculationAlgorithm.MaximumLikelihood)
        {
        }

        public override Pdf GetLikelihood(Fingerprint rss)
        {
            return new Pdf(GetPositionByFingerprint(rss));
        }

        private Coord GetPositionByFingerprint(Fingerprint fingerprint)
        {
            if (fingerprint.Rss.Count() != RadioMap.First().Fingerprints.First().Rss.Count())
                throw new ArgumentException("Different number of RSSI measurements");

            ProcessInfo stat = new ProcessInfo(ExtractName(), false);
            stat.StartNewStage();

            List<Tuple<Observations, double>> neighbours = new List<Tuple<Observations, double>>();
            foreach (Observations f in RadioMap)
            {
                double curDist = EuclideanDist(f.MeanRss, fingerprint);
                var farthest = Farthest(neighbours);
                if (neighbours.Count < K || farthest.Item2 > curDist)
                {
                    if (neighbours.Count == K)
                        neighbours.Remove(farthest);
                    neighbours.Add(new Tuple<Observations, double>(f, curDist));
                }
            }

            stat.FinishStage();
            stat.FinishProcess();

            return Mean(neighbours);
        }

        private Coord Mean(List<Tuple<Observations, double>> neighbours)
        {
            Coord res = new Coord(0, 0);
            double weightSum = 0;
            foreach (var n in neighbours)
            {
                var weight = 1 / n.Item2;
                weightSum += weight;
                res = res + weight * n.Item1.LocationLink.Center;
            }

            //neighbours.ForEach(c => { res = res + c.Item2 * c.Item1.LocationLink.Center; });
            return (1 / weightSum) * res;
        }

        private Tuple<Observations, double> Farthest(List<Tuple<Observations, double>> neighbours)
        {
            double maxValue = double.MinValue;
            Tuple<Observations, double> maxTuple = null;
            foreach (var t in neighbours)
                if (t.Item2 > maxValue)
                {
                    maxValue = t.Item2;
                    maxTuple = t;
                }
            return maxTuple;
        }

        /// <summary>
        /// Eucludian distance between fingerprints
        /// </summary>
        private double EuclideanDist(Fingerprint f1, Fingerprint f2)
        {
            if (f1.Rss.Count != f2.Rss.Count)
                throw new ArgumentException();

            double sqSum = 0;
            foreach (Device d in f1.Rss.Keys)
            {
                double diff = f1.Rss[d] - f2.Rss[d];
                sqSum += diff * diff;
            }

            return Math.Sqrt(sqSum);
        }
    }
}
