using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.Algorithms
{
    class PredictionResult
    {
        public List<PredictionResultItem> Locations { get; private set; }

        public PredictionResult()
        {
            Locations = new List<PredictionResultItem>();
        }

        public void add(AreaLocation location, double confidence)
        {
            PredictionResultItem pr = new PredictionResultItem(location, confidence);
            Locations.Add(pr);
        }

        public void add(IEnumerable<AreaLocation> locations, double confidence)
        {
            foreach (AreaLocation l in locations)
            {
                PredictionResultItem item = new PredictionResultItem(l, confidence);
                Locations.Add(item);
            }
        }

        public void addResult(PredictionResult res)
        {
            Locations.AddRange(res.Locations);
        }

        public override string ToString()
        {
            string content = "";
            foreach (PredictionResultItem item in Locations)
                content += "(" + item.Location + "," + item.Confidence + ")";

            return "{" + content + "}";
        }
    }
}
