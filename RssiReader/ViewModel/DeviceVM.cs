using LocalizationCore.SensorModel.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssiReader.ViewModel
{
    [Serializable]
    class DeviceVM
    {
        public int Number { get; set; }
        public Device Device { get; set; }

        public override string ToString()
        {
            return Number + ") " + Device;
        }
    }
}
