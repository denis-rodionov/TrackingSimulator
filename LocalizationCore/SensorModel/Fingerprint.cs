using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.SensorModel.Devices;

namespace LocalizationCore.SensorModel
{
    [Serializable]
    public class Fingerprint
    {
        public Dictionary<Device, double> Rss { get; set; }

        public Fingerprint()
        {
            Rss = new Dictionary<Device, double>();
        }

        public Fingerprint(Dictionary<Device, double> rssArray)
        {
            Rss = rssArray;
        }

        public void Add(Fingerprint rss)
        {
            foreach (var key in Rss.Keys.ToList())
                if (rss.Rss.Keys.Contains(key))
                    Rss[key] += rss.Rss[key];
                else
                    Rss[key] += 0;

            // creating new RSSI
            foreach (var key in rss.Rss.Keys.ToList())
                if (!Rss.Keys.Contains(key))
                    Rss.Add(key, rss.Rss[key]);
        }

        public void Divide(double val)
        {
            foreach (var key in Rss.Keys.ToList())
                Rss[key] /= val;
        }

        public override string ToString()
        {
            string res = "";
            foreach (var rss in Rss)
                res += rss.Key + ":" + rss.Value + "; ";
            return res;
        }

        public bool SameDevices(Fingerprint f)
        {
            foreach (var d in Rss.Keys)
                if (!f.Rss.ContainsKey(d))
                    return false;
            return true;
        }

        public bool Same(Fingerprint f)
        {
            if (!SameDevices(f))
                return false;

            foreach (var d in Rss.Keys)
                if (Rss[d] != f.Rss[d])
                    return false;

            return true;
        }
    }
}
