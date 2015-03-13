using LocalizationCore.Primitives;
using LocalizationCore.Primitives.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.Filtering
{
    public class ParticleFilter : BaseFilter
    {
        #region options

        public static double THRESHOLD_EFFECTIVE = 5;
        public static int PARTICLES_NUMBER = 100;

        const double PROCESS_NOICE = 1;
        const int VELOCITY_COUNT = 3;
        const int MEAS_COUNT = 20;

        #endregion

        #region data

        public List<Particle> Particles { get; set; }

        Random random = new Random();
        MyRandom myrandom = new MyRandom();
        Coord LastPosition { get; set; }
        
        //Vector LastVelosity { get; set; }

        protected LimitList<SVector> Velocities { get; set; }
        protected LimitList<Coord> Measurements { get; set; }

        #endregion

        #region Interface

        public ParticleFilter(SRect field, string name, LocalizationAlgorithm underlyingAlgorithm)
            : base(name, underlyingAlgorithm)
        {
            Measurements = new LimitList<Coord>(MEAS_COUNT);
            Particles = DistributeParticles(field);
            LastPosition = GetApproximation();
            Velocities = new LimitList<SVector>(VELOCITY_COUNT);
            Velocities.Add(new SVector(0, 0));
        }        

        public override Coord EstimateLocation(Coord realCoord)
        {
            var estimation = UnderlyingAlgorithm.EstimateLocation(realCoord);
            Measurements.Add(estimation);

            if (NeedResampling())
                Resampling(estimation);
            Prediction();
            Update(Measurements.Mean(), UnderlyingAlgorithm.CurrentVariance);

            CurrentEstimation = GetApproximation();
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

        private void Update(Coord estimation, double measNoice)
        {
            //measNoice = 2;
            double weightSum = 0;
            foreach (var p in Particles)
            {
                p.Weight = Gaussian.Func2Variance(p.Coord, estimation, measNoice, measNoice);
                weightSum += p.Weight;
            }

            if (weightSum == 0)
                throw new Exception();

            foreach (var p in Particles)
                p.Weight = p.Weight / weightSum;
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

        private void Resampling(Coord estimation)
        {
            foreach (var p in Particles)
                p.Coord = LastPosition + SVector.RandomVector(1, 1);
        }

        #endregion
    }
}
