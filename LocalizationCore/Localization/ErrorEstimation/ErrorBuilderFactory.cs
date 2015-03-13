using LocalizationCore.Localization.FingerprintingAlgorithms;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.ErrorEstimation
{
    class ErrorBuilderFactory
    {
        public static BaseErrorMapBuilder Create(AccuracyAlgorithm alg, RadioMap map = null,
                                                 IEnumerable<Device> devices = null,
                                                 Func<Fingerprint, Coord> EstimationFunc = null)
        {
            BaseErrorMapBuilder res = null;

            switch (alg)
            {
                case AccuracyAlgorithm.LeaveOut:
                    res = new LeaveoutErrorMapBuilder(map, EstimationFunc);
                    break;
                case AccuracyAlgorithm.DistBased:
                    res = new DistanceErrorMapBuilder(map, devices);
                    break;
                case AccuracyAlgorithm.Clustering:
                    res = new ClusteringErrorMapBuilder(map);
                    break;
                default:
                    throw new Exception("Unknown accuracy estimation algorithm!");
            }

            return res;
        }
    }
}
