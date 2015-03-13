using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.SensorModel.Devices;

namespace LocalizationCore.SensorModel
{
    public class RssSensorData
    {
        public Dictionary<Device, double> Rss { get; set; }

        public RssSensorData()
        {
            Rss = new Dictionary<Device, double>();
        }

        public void Add(RssSensorData rss)
        {
            if (rss.Rss.Keys.Count() != Rss.Keys.Count())
                throw new ArgumentException();
            else foreach (var key in Rss.Keys.ToList())
                Rss[key] += rss.Rss[key];
        }

        public void Divide(double val)
        {
            foreach (var key in Rss.Keys.ToList())
                Rss[key] /= val;
        }
    }
}
