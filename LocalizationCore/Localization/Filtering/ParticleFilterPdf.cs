using LocalizationCore.Localization.Map;
using LocalizationCore.Primitives;
using LocalizationCore.Primitives.Algorithms;
using LogProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.Filtering
{
    public class ParticleFilterPdf : BaseFilter
    {
        #region options

        public static double THRESHOLD_EFFECTIVE = 1;
        public static double PARTICLES_NUMBER = 100;

        const double PROCESS_NOICE = 1;
        const int VELOCITY_COUNT = 10;
        const int MEAS_COUNT = 5;

        #endregion

        #region data

        public List<Particle> Particles { get; set; }

        Random random = new Random();
        MyRandom myrandom = new MyRandom();
        Coord LastPosition { get; set; }
        
        //Vector LastVelosity { get; set; }

        protected LimitList<SVector> Velocities { get; set; }
        protected LimitList<Pdf> Measurements { get; set; }

        #endregion

        #region Interface

        public ParticleFilterPdf(SRect field, string name, LocalizationAlgorithm underlyingAlgorithm)
            : base(name, underlyingAlgorithm)
        {
            Measurements = new LimitList<Pdf>(MEAS_COUNT);
            Particles = DistributeParticles(field);
            LastPosition = GetApproximation();
            Velocities = new LimitList<SVector>(VELOCITY_COUNT);
            Velocities.Add(new SVector(0, 0));
        }        

        public override Coord EstimateLocation(Coord realCoord)
        {
            UnderlyingAlgorithm.EstimateLocation(realCoord);
            var estimation = UnderlyingAlgorithm.LastLikelihood;

            Measurements.Add(estimation);

            if (NeedResampling())
                Resampling(estimation);
            Prediction();
            Update(Measurements.Average(), UnderlyingAlgorithm.CurrentVariance);

            LastLikelihood = Measurements.Average();
            CurrentEstimation = GetApproximation();
            //CurrentEstimation = LastLikelihood.MeanK(10);     // just for fun
            EstimateError(realCoord);

            UpdateState(CurrentEstimation);

            return CurrentEstimation;
        }

        private bool NeedResampling()
        {
            double sqSum = 0;
            foreach (var p in Particles)
                sqSum += p.Weight * p.Weight;
            var eff = 1 / (sqSum);

            return eff < THRESHOLD_EFFECTIVE;
        }

        #endregion

        #region implementation

        private void UpdateState(Coord CurrentEstimation)
        {
            Velocities.Add(new SVector(LastPosition, CurrentEstimation));

            LastPosition = CurrentEstimation;
        }

        private List<Particle> DistributeParticles(SRect area)
        {            
            var res = new List<Particle>();

            for (int i = 0; i < PARTICLES_NUMBER; i++)
                res.Add(new Particle() { Coord = RandomCoord(area), Weight = 1 / (double)PARTICLES_NUMBER });

            return res;
        }

        private Coord RandomCoord(SRect rect)
        {
            var x = random.NextDouble() * rect.Width + rect.TopLeft.X;
            var y = random.NextDouble() * rect.Height + rect.TopLeft.Y;

            return new Coord(x, y);
        }

        private Coord GetApproximation()
        {
            var sum = new Coord(0, 0);
            foreach (var p in Particles)
                sum += p.Weight * p.Coord;            

            return sum;
        }

        private void Update(Pdf estimation, double measNoice)
        {
            //measNoice = 2;
            double weightSum = 0;
            foreach (var p in Particles)
            {
                //p.Weight = Gaussian.Func2Variance(p.Coord, estimation, measNoice, measNoice);
                p.Weight = estimation.Get(p.Coord);
                weightSum += p.Weight;
            }

            if (weightSum == 0)     // particles flew away
                Resampling(estimation);

            NormalizeParticleWeights();
        }

        private void NormalizeParticleWeights()
        {
            double sum = 0;
            foreach (var p in Particles)
                sum += p.Weight;

            foreach (var p in Particles)
                p.Weight = p.Weight / sum;
        }

        protected virtual void Prediction()
        {
            SVector vel = Velocities.Mean();
            foreach (var p in Particles)
            {
                p.Coord = p.Coord + Predict(vel);
            }
        }

        protected static SVector Predict(SVector vel)
        {
            return SVector.RandomVector(0.3, 0.1) + vel;
        }        

        private Coord ProcessNoise()
        {
            return new Coord(myrandom.NextDouble(PROCESS_NOICE), myrandom.NextDouble(PROCESS_NOICE));
        }

        #region

        int resampling = 0;
        private void Resampling(Pdf estimation)
        {
            Logger.Log("Resampling #" + (++resampling));
            //CircleResampling(estimation);
            PdfResampling(estimation);
        }

        private void PdfResampling(Pdf estimation)
        {
            var rand = new MyRandom();
            var tempPdf = estimation.Clone().GetLocations();
            var parWeight = 1 / (double)PARTICLES_NUMBER;

            foreach (var p in Particles)
            {
                var item = tempPdf.OrderByDescending(d => d.Value).First();
                p.Coord = item.Key.Center + rand.NextCoord(LocationMap.Instance.LocationSize / 2);
                p.Weight = estimation[item.Key];
                tempPdf[item.Key] -= parWeight / 10;
            }

            NormalizeParticleWeights();
        }

        private void CircleResampling(Pdf estimation)
        {
            foreach (var p in Particles)
            {
                p.Coord = LastPosition + SVector.RandomVector(1, 1);
                p.Weight = estimation.Get(p.Coord);
            }
            NormalizeParticleWeights();
        }

        #endregion

        #endregion
    }
}
