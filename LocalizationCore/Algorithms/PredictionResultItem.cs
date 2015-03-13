
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.Algorithms
{
    class PredictionResultItem
    {
        public PredictionResultItem(AreaLocation location, double confidence)
        {
            Location = location;
            Confidence = confidence;
        }

        public AreaLocation Location { get; set; }
        public double Confidence { get; set; }
    }
}
