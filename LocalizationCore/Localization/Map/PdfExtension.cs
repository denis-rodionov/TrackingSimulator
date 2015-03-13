using LocalizationCore.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization
{
    static class PdfExtension
    {
        public static Coord MaximumLikelihood(this Pdf pdf)
        {
            return pdf.GetLocations().OrderByDescending(p => p.Value).First().Key.Center;
        }

        public static Coord MaximumLikelihood(this Pdf pdf, int k)
        {
            return Average(pdf.GetLocations().OrderByDescending(p => p.Value).Take(k).Select(p => p.Key).AsEnumerable());
        }

        public static Coord Mean(this Pdf pdf)
        {
            return WeightedAverage(pdf.GetLocations());
        }

        public static Coord MeanK(this Pdf pdf, int k)
        {
            return WeightedAverage(pdf.GetLocations().OrderByDescending(p => p.Value).Take(k));
        }

        private static Coord WeightedAverage(IEnumerable<KeyValuePair<Location, double>> pdf)
        {
            Coord res = new Coord(0, 0);
            foreach (var p in pdf)
                res += (p.Value * p.Key.Center);

            double sum = pdf.Sum(p => p.Value);

            return res = res / sum;
        }

        private static Coord Average(IEnumerable<Location> locations)
        {
            Coord res = new Coord(0, 0);
            foreach (var l in locations)
                res += l.Center;
            res = res / locations.Count();
            return res;
        }

        public static Pdf Average(this IEnumerable<Pdf> pdfs)
        {
            var res = new Pdf();
            foreach (var pdf in pdfs)
                res += pdf;
            res.Normalize();
            return res;
        }
    }
}
