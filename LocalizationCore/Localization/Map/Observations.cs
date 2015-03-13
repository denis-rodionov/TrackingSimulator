using LocalizationCore.Primitives;
using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization
{
    public class Observations
    {
        public ObservableCollection<Fingerprint> Fingerprints { get; private set; }

        private Fingerprint cashedMean = null;
        public Fingerprint MeanRss
        {
            get
            {
                if (cashedMean != null)
                    return cashedMean;

                if (Fingerprints.Count() == 0)
                    throw new Exception("Cannot calculate mean: no measurements added to the location");

                Fingerprint sum = null;
                foreach (var meas in Fingerprints)
                    if (sum == null)
                        sum = meas;
                    else
                        sum.Add(meas);
                sum.Divide(Fingerprints.Count());
                cashedMean = sum;
                return sum;
            }
        }

        public Coord Accuracy { get; set; }

        public Location LocationLink { get; set; }

        public Observations(Location location)
        {
            Fingerprints = new ObservableCollection<Fingerprint>();
            Accuracy = null;
            LocationLink = location;
        }

        public void AddCalibrationPoint(Fingerprint fingerprint)
        {
            cashedMean = null;
            if (Fingerprints.Any() && Fingerprints.First().Rss.Count() != fingerprint.Rss.Count())
                throw new ArgumentException("The fingerprint has different size with others. Cannot add!");

            Fingerprints.Add(fingerprint);
        }

        public override string ToString()
        {
            string res = "";
            foreach (Device d in MeanRss.Rss.Keys)
                res += d.Name + ": " + MeanRss.Rss[d].ToString("#.00") + ", ";
            return res + "}";
        }

        public double GetValue(Device device)
        {
            if (Fingerprints.Count() == 0)
                return 0;

            double res;
            if (MeanRss.Rss.TryGetValue(device, out res) == false)
                res = 0;

            return res;
        }
    }
}
