using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel.Sensors;
using LocalizationCore.Localization.FingerprintingAlgorithms;
using LocalizationCore.Localization.Map;

namespace LocalizationCore.Localization
{
    public class DataFusion : LocalizationAlgorithm
    {
        List<LocalizationAlgorithm> algs = new List<LocalizationAlgorithm>();

        public AccuracyAlgorithm AccuracyAlgorithm { get; private set; }
        public string Name { get; set; }

        public DataFusion(string name, AccuracyAlgorithm accuracyAlgorithm, params LocalizationAlgorithm[] algorithms)
            : base(name)
        {
            Name = name;
            AccuracyAlgorithm = accuracyAlgorithm;
            algs.AddRange(algorithms);

            foreach (var alg in algs)
                if (alg is Fingerprinting)
                    ((Fingerprinting)alg).BuildAccuracyMap(accuracyAlgorithm);
        }

        public override Coord EstimateLocation(Coord realCoord)
        {
            var errors = new Dictionary<LocalizationAlgorithm, double>();
            double sumWeight = 0;
            foreach (var alg in algs)                   // low-level algorithms stage
            {
                alg.EstimateLocation(realCoord);
                var err = alg.GetPriorError(realCoord, AccuracyAlgorithm).X;
                errors.Add(alg, err);
                sumWeight += 1 / err;
            }

            var pdf = new Pdf();

            foreach (var loc in LocationMap.Instance)
            {
                double p = 0;
                foreach (var alg in algs)
                    p += alg.LastLikelihood[loc] / errors[alg];
                pdf[loc] = p / sumWeight;
            }
            pdf.Normalize();
            LastLikelihood = pdf;

            CurrentEstimation = pdf.MeanK(10);
            EstimateError(realCoord);
            return CurrentEstimation;
            
            //Coord res = new Coord(0,0);
            //Coord w = new Coord(0,0);

            //var loc = realCoord;
            //var weightedCoords = from alg in algs
            //                     select new Coord(
            //                         alg.CurrentEstimation.X / alg.GetPriorError(loc, AccuracyAlgorithm).X,
            //                         alg.CurrentEstimation.Y / alg.GetPriorError(loc, AccuracyAlgorithm).Y);


            //foreach (var coord in weightedCoords)
            //    res = res + coord;

            //foreach (var alg in algs)
            //{
            //    w.X = w.X + 1 / alg.GetPriorError(loc, AccuracyAlgorithm).X;
            //    w.Y = w.Y + 1 / alg.GetPriorError(loc, AccuracyAlgorithm).Y;
            //}

            //CurrentEstimation = new Coord(res.X / w.X, res.Y / w.Y);

            //EstimateError(realCoord);

            //return CurrentEstimation;
        }

        public void AddSource(LocalizationAlgorithm source)
        {
            algs.Add(source);
        }

        public override Coord GetPriorError(Coord coord, AccuracyAlgorithm alg)
        {
            throw new NotImplementedException();    // no prior accuracy
        }
    }
}
