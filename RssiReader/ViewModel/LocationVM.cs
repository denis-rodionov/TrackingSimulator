using LocalizationCore.Localization;
using LocalizationCore.SensorModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssiReader.ViewModel
{
    [Serializable]
    class LocationVM : INotifyPropertyChanged
    {
        public Observations OriginalLocation { get; set; }

        public IEnumerable<DeviceVM> DevicesRef { get; set; }

        public IEnumerable<FingerprintVM> Fingerprints
        {
            get
            {
                return OriginalLocation.Fingerprints.Select(f => new FingerprintVM() { OriginalFingerprint = f, DevicesRef = DevicesRef }).ToList();
            }
        }

        public void AddFingerprint(Fingerprint fingerprint)
        {
            OriginalLocation.Fingerprints.Add(fingerprint);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Fingerprints"));
        }

        public void RemoveFingerprint(FingerprintVM fingerprint)
        {
            OriginalLocation.Fingerprints.Remove(fingerprint.OriginalFingerprint);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Fingerprints"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
    }
}
