using LocalizationCore.BuildingModel;
using LocalizationCore.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.Filtering
{
    public class WallParticleFilter : ParticleFilter
    {
        public WallParticleFilter(SRect field, string name, LocalizationAlgorithm underlyingAlgorithm)
            : base(field, name, underlyingAlgorithm)
        {
        }

        protected override void Prediction()
        {
            SVector vel = Velocities.Mean();
            foreach (var p in Particles)
            {
                //var noise = ProcessNoise();
                //var v = vel * ((random.NextDouble() - 0.5) * 10);
                var temp = p.Coord + Predict(vel);

                Wall wall = Building.Instance.Floor.IntersectWall(p.Coord, temp);
                if (wall != null)
                    p.Coord = SVector.Intersection(temp, p.Coord, wall.Begin, wall.End);
                else
                    p.Coord = temp;
            }
        }
    }
}
