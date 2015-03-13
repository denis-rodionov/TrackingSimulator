using LocalizationCore.BuildingModel;
using LocalizationCore.Localization.FingerprintingAlgorithms;
using LocalizationCore.PersonModel;
using LocalizationCore.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization
{
    public class GpsLocalization : LocalizationAlgorithm
    {
        public const double NORMAL_ERROR = 1;
        public const double HUGE_ERROR = 10;
        public const string NAME = "GPS";

        MyRandom random = new MyRandom();
        FloorInstance Floor { get; set; }

        public GpsLocalization(FloorInstance floorInstance)
            : base(NAME)
        {
            Floor = floorInstance;
        }

        public override Coord GetPriorError(Coord coord, AccuracyAlgorithm alg)
        {
            if (coord == null)
                throw new ArgumentException();

            var error = double.MaxValue;
            if (OpenArea(coord))
                error = 0.1;
            //else
            //    error = 1;

            return new Coord(error, error);
        }

        private bool OpenArea(Coord coord)
        {
            return Floor == FloorInstance.Hospital && (coord.Y <= 2 || coord.Y >= 8);
        }

        public override Coord EstimateLocation(Coord realCoord)
        {
            var error = HUGE_ERROR;
            if (OpenArea(realCoord))
                error = NORMAL_ERROR;

            var res = new Coord(realCoord.X + SimpleRNG.GetNormal(0, error), realCoord.Y + SimpleRNG.GetNormal(0, error));

            LastLikelihood = new Pdf(res);
            CurrentEstimation = res;
            EstimateError(realCoord);
            return res;
        }
    }
}
