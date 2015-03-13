using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.Primitives
{
    class MyRandom
    {
        Random random = new Random();

        public MyRandom()
        {
        }

        /// <summary>
        /// Returns values uniformly distributed from -disp to disp
        /// </summary>
        public double NextDouble(double disp)
        {
            return (random.NextDouble() - 0.5) * 2 * disp;
        }

        public Coord NextCoord(double offset)
        {
            return new Coord(NextDouble(offset), NextDouble(offset));
        }
    }
}
