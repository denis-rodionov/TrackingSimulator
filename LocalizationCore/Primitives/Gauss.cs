using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Primitives
{
    public class Gauss
    {
        public static double OverlappingCoefficient(double mu1, double mu2, double sigma1, double sigma2)
        {
            double x1, x2;
            Intersection(mu1, mu2, sigma1, sigma2, out x1, out x2);

            double cdf11 = CDF(x1, mu1, sigma1);
            double cdf12 = CDF(x2, mu1, sigma1);
            double cdf21 = CDF(x1, mu2, sigma2);
            double cdf22 = CDF(x2, mu2, sigma2);

            double res;
            if (sigma1 < sigma2)
                res = cdf11 + cdf22 - cdf21 + 1 - cdf12;
            else
                res = cdf21 + cdf12 - cdf11 + 1 - cdf22;

            return res;
        }

        public static void Intersection(double mu1, double mu2, double sigma1, double sigma2, out double x1, out double x2)
        {
            double mu1_2 = mu1 * mu1;
            double mu2_2 = mu2 * mu2;
            double sigma1_2 = sigma1 * sigma1;
            double sigma2_2 = sigma2 * sigma2;

            double a = sigma1_2 - sigma2_2;
            double b = 2 * mu1 * sigma2_2 - 2 * mu2 * sigma1_2;
            double c = mu2_2 * sigma1_2 - mu1_2 * sigma2_2 - sigma1_2 * sigma2_2 * Math.Log(sigma1_2 / sigma2_2);

            double d = b * b - 4 * a * c;
            x1 = (-b - Math.Sqrt(d)) / (2 * a);
            x2 = (-b + Math.Sqrt(d)) / (2 * a);

            if (x1 > x2)
            {
                double temp = x1;
                x1 = x2;
                x2 = temp;
            }

            //Console.WriteLine("x1 = {0}; x2 = {1}; d = {2}", x1, x2, d);
        }

        public static double CDF(double x, double mu, double sigma)
        {
            double arg = (x - mu) / sigma;
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (arg < 0)
                sign = -1;
            arg = Math.Abs(arg) / Math.Sqrt(2.0);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * arg);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-arg * arg);

            return 0.5 * (1.0 + sign * y);
        }
    }
}
