using LocalizationCore.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.Filtering
{
    public class Particle
    {
        public Coord Coord { get; set; }
        public double Weight { get; set; }

        public override string ToString()
        {
            return Coord.ToString() + " : " + Weight.ToString("F");
        }
    }
}
