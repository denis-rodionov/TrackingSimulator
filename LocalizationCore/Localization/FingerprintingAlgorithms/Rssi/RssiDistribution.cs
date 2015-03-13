using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.FingerprintingAlgorithms.Rssi
{
    /// <summary>
    /// Histogram approximation of signal distribution
    /// </summary>
    class RssiDistribution
    {
        double Min { get; set; }
        double Max { get; set; }
        
        int binCount;              // number of columns
        double[] histograms;
        double binWidth;

        public RssiDistribution(int binCount, double min, double max)
        {
            this.Min = min;
            this.Max = max;
            this.binCount = binCount;
            histograms = new double[binCount];
            binWidth = (Max - Min) / binCount;            
        }

        public double Multiply(double rssi)
        {
            return histograms[GetIndex(rssi)];
        }

        private int GetIndex(double rssi)
        {
            int index;
            if (rssi < Min)
                index = 0;
            else if (rssi > Max)
                index = binCount - 1;
            else
                index = (int)((rssi - Min) / binWidth);
            return index;
        }

        public void AddPoint(double rssi)
        {
            histograms[GetIndex(rssi)] += 1;
        }

        public void Normalize()
        {
            var sum = histograms.Sum();
            for (int i = 0; i < binCount; i++)
                histograms[i] /= sum; 
        }

        public override string ToString()
        {
            var res = string.Empty;
            foreach (var val in histograms)
                res += val + "; ";
            return res;
        }
    }
}
