using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogProvider.ProcessInfo;
using LocalizationCore.BuildingModel;
using LocalizationCore.PersonModel;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Devices;
using LocalizationCore.SensorModel.Sensors;
using LocalizationCore.Localization.ErrorEstimation;
using LocalizationCore.Localization.Map;

namespace LocalizationCore.Localization.FingerprintingAlgorithms
{
    public enum AccuracyAlgorithm { None, LeaveOut, DistBased, Clustering };
    public enum CalculationAlgorithm { MaximumLikelihood, KMax, Mean, MeanK };

    public abstract class Fingerprinting : LocalizationAlgorithm
    {
        const double MEAS_COUNT = 50;
        const double STD_ACCURACY = 1;
        const string MAP_FILE_NAME = "rssi_map.dat";
        public static int MAX_K = 4;
        
        //List<Fingerprint> map;
        protected RadioMap RadioMap { get; set; }

        public string Name { get; set; }
        public RssiSensor Sensor { get; set; }
        
        public CalculationAlgorithm Calculator { get; set; }

        public Fingerprinting(string name, RssiSensor sensor, CalculationAlgorithm calc) : base(name)
        {
            Sensor = sensor;
            RadioMap = new RadioMap();
            Name = name;
            BuildRssiMap(Building.Instance.Floor, Sensor);
            Calculator = calc;
        }

        /// <summary>
        /// Online phase of fingerprinting technique
        /// Location estimation by means of NN algorithm
        /// </summary>
        /// <returns></returns>
        public override Coord EstimateLocation(Coord realCoord)
        {
            var rss = Sensor.GetMeasurements(realCoord);

            CurrentEstimation = GetPosition(rss);
            EstimateError(realCoord);

            return CurrentEstimation;
        }

        private Coord GetPosition(Fingerprint rss)
        {
            LastLikelihood = GetLikelihood(rss);
            LastLikelihood.Normalize();
            return GetCoord(LastLikelihood);
        }

        public abstract Pdf GetLikelihood(Fingerprint fingerprint);

        //public abstract Coord GetPositionByFingerprint(Fingerprint fingerprint);

        protected string ExtractName()
        {
            return "Estimate location";
        }

        #region Service

        public void PrintStatistics()
        {
            ProcessStatistics.GetStatistics(ExtractName()).Log();
            ProcessStatistics.GetStatistics("BuildRssiMap").Log();
            var t = ProcessStatistics.GetStatistics("BuildAccuracyMap");
            if (t.StageStatistics != null)
                t.Log();
        }

        public void BuildRssiMap(Floor floor, RssiSensor rss)
        {
            MyRandom rand = new MyRandom();
            ProcessInfo stat = new ProcessInfo("BuildRssiMap");
            stat.StartNewStage();

            foreach (var loc in LocationMap.Instance)
                for (int t = 0; t < MEAS_COUNT; t++)
                    RadioMap.Add(loc, rss.GetMeasurements(loc.Center + rand.NextCoord(LocationMap.Instance.LocationSize / 2)));

            stat.FinishStage();
            stat.FinishProcess();
        }

        #endregion

        public void BuildAccuracyMap(AccuracyAlgorithm alg)
        {
            ProcessInfo stat = new ProcessInfo("BuildAccuracyMap");
            stat.StartNewStage();

            var builder = ErrorBuilderFactory.Create(alg, RadioMap, Sensor.GetAccessPoints(), GetPosition);
            builder.BuildAccuracyMap();

            stat.FinishStage();
            stat.FinishProcess();
        }
        
        public override Coord GetPriorError(Coord coord = null, AccuracyAlgorithm alg = AccuracyAlgorithm.None)
        {
            Coord res;

            if (alg == AccuracyAlgorithm.None)
                res = new Coord(STD_ACCURACY, STD_ACCURACY);
            else
            {
                var loc = LocationMap.Instance.FindLocation(coord);
                if (loc == null)
                    throw new Exception("Location cannot be out of the map!");
                else
                    res = RadioMap[loc].Accuracy;
            }
             
            if (res == null)
                throw new Exception("Accuracy map was not built!");
            return res;
        }
        
        private Coord GetCoord(Pdf pdf)
        {
            switch (Calculator)
            {
                case CalculationAlgorithm.MaximumLikelihood:
                    return pdf.MaximumLikelihood();
                case CalculationAlgorithm.KMax:
                    return pdf.MaximumLikelihood(MAX_K);
                case CalculationAlgorithm.Mean:
                    return pdf.Mean();
                case CalculationAlgorithm.MeanK:
                    return pdf.MeanK(MAX_K);
                default:
                    throw new Exception("Unknown calculator");
            }
        }
    }
}
