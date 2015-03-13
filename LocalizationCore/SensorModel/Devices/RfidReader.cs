using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.Primitives;

namespace LocalizationCore.SensorModel.Devices
{
    [Serializable]
    class RfidReader : AccessPoint
    {
        public const double RSSI_0 = 30;

        public RfidReader(string name, Coord coord)
            : base(name, coord, RSSI_0)
        {
        }
    }
}
