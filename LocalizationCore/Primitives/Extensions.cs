using LocalizationCore.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Primitives
{
    public static class Extensions
    {
        public static double Mean(this Dictionary<int, double> dict)
        {
            return MomentCalculator.Mean(dict.Values);
        }

        public static double Mean(this IEnumerable<double> arr)
        {
            return MomentCalculator.Mean(arr);
        }

        public static Coord Mean(this IEnumerable<Coord> coords)
        {
            Coord res = new Coord(0, 0);
            foreach (var c in coords)
                res = res + c;
            return (1 / (double)coords.Count()) * res;
        }

        public static SVector Mean(this IEnumerable<SVector> vectors)
        {
            SVector res = new SVector(0, 0);
            foreach (var v in vectors)
                res = res + v;

            res = res / vectors.Count();
            return res;
        }

        public static double Variance(this Dictionary<int, double> dict)
        {
            return MomentCalculator.Variance(dict.Values);
        }

        public static double Variance(this IEnumerable<double> arr)
        {
            return MomentCalculator.Variance(arr);
        }

        public static double Variance(this Dictionary<double, double> dict)
        {
            return MomentCalculator.Variance(dict.Values);
        }

        public static double Less(this Dictionary<int, double> dict, double val)
        {
            int counter = 0;
            foreach (var item in dict)
                if (item.Value < val)
                    counter++;

            return counter / (double)dict.Count;
        }

        public static Dictionary<double, double> Smooth(this Dictionary<double, double> dict, int window)
        {
            var keys = dict.Keys.ToList();
            Dictionary<int, double> temp = new Dictionary<int,double>();
            for (int i = 1; i <= dict.Keys.Count(); i++)
                temp.Add(i, dict[keys[i-1]]);

            temp = temp.Smooth(window);

            Dictionary<double, double> res = new Dictionary<double,double>();
            foreach (var el in temp)
                res.Add(keys[el.Key-1], el.Value);

            return res;
        }

        /// <summary>
        /// Smooth values of the dictionary (Average filter)
        /// </summary>
        /// <param name="dict">Keys of the dictionary must start from 1 and the last key must be count of all keys</param>
        /// <returns>Creates new dictionary with smoothed results</returns>
        public static Dictionary<int, double> Smooth(this Dictionary<int, double> dict, int window)
        {
            Dictionary<int, double> res = new Dictionary<int, double>();
            int count = dict.Keys.Count();

            for (int i = 1; i <= count; i++)
            {
                double sum = 0;
                int spareCount = 0;
                for (int j = i - window; j <= i + window; j++)
                    if (j > 0 && j <= count)
                        sum += dict[j];
                    else
                        spareCount++;

                res[i] = sum / (2 * window + 1 - spareCount);
            }
            return res;
        }
    }
}
