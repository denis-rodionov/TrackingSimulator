using LocalizationCore.SensorModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RssiReader.ViewModel
{
    class FingerprintVM
    {
        public Fingerprint OriginalFingerprint { get; set; }

        public IEnumerable<DeviceVM> DevicesRef { get; set; }

        public override string ToString()
        {
            string res = "";

            foreach (var dev in DevicesRef)
                res += dev.Number + ") " + GetValue(dev) + "\t";

            return res;
        }

        private double? GetValue(DeviceVM dev)
        {
            var q = OriginalFingerprint.Rss.Where(v => v.Key.Name == dev.Device.Name);
            if (q.Count() == 0)
                return null;
            else
                return q.Single().Value;
        }
    }
}
