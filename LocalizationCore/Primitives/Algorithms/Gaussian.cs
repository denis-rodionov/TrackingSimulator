using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Primitives.Algorithms
{
    public class Gaussian
    {
        /// <summary>
        /// Two dimmentional Gaussian function
        /// </summary>
        /// <param name="center"></param>
        /// <param name="sigmaX"></param>
        /// <param name="sigmaY"></param>
        /// <returns></returns>
        public static double Func2(Coord arg, Coord center, double sigmaX, double sigmaY)
        {
            return Func2Variance(arg, center, sigmaX * sigmaX, sigmaY * sigmaY);
        }

        public static double Func2Variance(Coord arg, Coord center, double varianceX, double varianceY)
        {
            var x = arg.X - center.X;
            var y = arg.Y - center.Y;
            return Math.Exp(-( x * x / (2 * varianceX) + (y * y) / (2 * varianceY)));
        }
    }
}
