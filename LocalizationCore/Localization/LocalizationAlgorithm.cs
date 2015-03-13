using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.Primitives;
using LocalizationCore.Localization.FingerprintingAlgorithms;

namespace LocalizationCore.Localization
{
    public abstract class LocalizationAlgorithm
    {
        const int CURRENT_ACTUAL = 5;
        const int STD_VARIANCE = 2;

        public Dictionary<int, double> ErrorHistory { get; private set; }

        private Coord currentEstimation = null;
        private Coord lastEstimation = null;
        
        public string Name { get; set;  }

        public LocalizationAlgorithm(string name)
        {
            Name = name;
            this.ErrorHistory = new Dictionary<int,double>();
        }

        public abstract Coord GetPriorError(Coord coord, AccuracyAlgorithm alg);

        public abstract Coord EstimateLocation(Coord realCoord);

        /// <summary>
        /// Current estimated position
        /// </summary>
        public Coord CurrentEstimation
        {
            get { return currentEstimation; }
            set
            {
                lastEstimation = currentEstimation;
                currentEstimation = value;
            }
        }

        /// <summary>
        /// Derived classes are responsible for setting LastLikelihood
        /// </summary>
        public Pdf LastLikelihood { get; set; }

        /// <summary>
        /// Last estimated position (before current)
        /// </summary>
        public Coord LastEstimation
        {
            get
            {
                if (lastEstimation == null)
                    lastEstimation = currentEstimation;
                return lastEstimation;
            }
            private set { lastEstimation = value; }
        }

        public void EstimateError(Coord realCoord)
        {
            int lastNumber = ErrorHistory.Any() ? ErrorHistory.Keys.Max() : 0;
            ErrorHistory.Add(lastNumber + 1, Coord.Distance(realCoord, CurrentEstimation));
        }

        public double MeanError()
        {
            return ErrorHistory.Mean();
        }

        public double ErrorVariance()
        {
            return ErrorHistory.Variance();
        }

        /// <summary>
        /// Smoothed error history
        /// </summary>
        /// <param name="filterWindow"></param>
        /// <returns></returns>
        public Dictionary<int, double> SmoothError(int filterWindow)
        {
            return ErrorHistory.Smooth(filterWindow);
        }

        /// <summary>
        /// Cumulative probability function from error history
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public Dictionary<double, double> CDF(double maxValue)
        {
            var res = new Dictionary<double, double>();
            var count = 100;    // discretization
            var step = maxValue / count;
            
            for (int i = 0; i < count; i++)
            {
                var err = i * step;
                res.Add(i * maxValue / count, ErrorHistory.Less(err));
            }

            return res;
        }

        public double CurrentVariance
        {
            get
            {
                if (ErrorHistory.Count > CURRENT_ACTUAL)
                {
                    var temp = new Dictionary<int, double>();
                    for (int i = ErrorHistory.Last().Key, j = 0; j < 5; i--, j++)
                        temp.Add(j, ErrorHistory[i]);
                    return temp.Variance();
                }
                else if (ErrorHistory.Count < 2)
                    return STD_VARIANCE;
                else
                    return ErrorHistory.Variance();
            }
        }
    }
}
