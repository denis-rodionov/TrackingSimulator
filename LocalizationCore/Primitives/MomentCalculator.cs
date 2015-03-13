using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Primitives
{
    class MomentCalculator
    {
        public static double Mean(IEnumerable<double> arr)
        {
            int count = 0;
            double sum = 0;
            foreach (var item in arr)
            {
                sum += item;
                count++;
            }
            return sum / count;
        }

        public static double Variance(IEnumerable<double> arr)
        {
            double mean = arr.Mean();
            double sum = 0;
            int count = 0;

            foreach (var item in arr)
            {
                sum += (item - mean) * (item - mean);
                count++;
            }
            return sum / count;
        }
    }
}
