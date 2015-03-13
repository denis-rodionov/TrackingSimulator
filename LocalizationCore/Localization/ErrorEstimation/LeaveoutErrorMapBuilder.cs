using LocalizationCore.Localization.FingerprintingAlgorithms;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.ErrorEstimation
{
    public class LeaveoutErrorMapBuilder : BaseErrorMapBuilder
    {
        const double FITTING_COEF_LEAVE = 1.5;

        RadioMap Map { get; set; }

        Func<Fingerprint, Coord> EstimationFunc { get; set; }
        
        public LeaveoutErrorMapBuilder(RadioMap map, Func<Fingerprint, Coord> estimationFunc)
        {
            Map = map;
            EstimationFunc = estimationFunc;
        }

        public override void BuildAccuracyMap()
        {
            var fList = Map.ToList();
            foreach (var item in fList)
                EstimateAccuracy(item);
        }

        /// <summary>
        /// Leave Out methods for accuracy estimation
        /// </summary>
        /// <param name="observations"></param>
        private void EstimateAccuracy(Observations observations)
        {
            var actualMap = new RadioMap();
            actualMap.Add(Map);

            Map.RemoveObservations(observations);

            double error = 0;
            foreach (var f in observations.Fingerprints)
            {
                var estCoord = EstimationFunc(f);
                error += Coord.Distance(estCoord, observations.LocationLink.Center);
            }

            error /= (observations.Fingerprints.Count() * FITTING_COEF_LEAVE);
            observations.Accuracy = new Coord(error, error);

            Map = actualMap;    // restore map;
        }
    }
}
