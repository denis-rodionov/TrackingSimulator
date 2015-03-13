using LocalizationCore.Localization.Locations;
using LocalizationCore.Localization.Map;
using LocalizationCore.Primitives;
using LogProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization
{
    public class Pdf
    {

        /// <summary>
        /// Approximation of pdf function
        /// </summary>
        Dictionary<Location, double> pdf;

        LocationMap Map;

        #region Init

        public Pdf()
        {
            Init();
        }

        public Pdf(Coord init)
        {
            Init();
            var loc = Map.FindLocation(init);
            if (loc != null)
                pdf.Add(loc, 1);
        }

        private void Init()
        {
            pdf = new Dictionary<Location, double>();
            Map = LocationMap.Instance;
        }

        #endregion

        public double Get(Coord coord)
        {
            var loc = Map.FindLocation(coord);
            if (loc == null)
                return 0;
            else
                return Get(loc);
        }

        public double Get(Location location)
        {
            if (pdf.ContainsKey(location))
                return pdf[location];
            else
                return 0;
        }

        public double this[Location location]
        {
            get { return Get(location); }
            set { Set(location, value); }
        }

        public Dictionary<Location, double> GetLocations() { return pdf; }
        
        public void Set(Coord coord, double value)
        {
            var loc = Map.FindLocation(coord);

            if (loc != null)
                Set(loc, value);
            else
                Logger.Log("Set(): location was not found!", LoggingLevel.Debug);
        }

        public void Set(Location location, double value)
        {
            if (pdf.ContainsKey(location))
                pdf[location] = value;
            else
                pdf.Add(location, value);
        }

        public void Normalize()
        {
            double sum = pdf.Sum(p => p.Value);
            for (var i = 0; i < pdf.Keys.Count; i++)
                pdf[pdf.Keys.ElementAt(i)] /= sum;
        }

        public Pdf Clone()
        {
            var res = new Pdf();
            res.pdf = new Dictionary<Location, double>(pdf);
            return res;
        }

        public static Pdf operator +(Pdf pdf1, Pdf pdf2)
        {
            Pdf res = new Pdf();
            foreach (var loc in res.Map)
                res[loc] = pdf1[loc] + pdf2[loc];
            return res;
        }        
    }
}
