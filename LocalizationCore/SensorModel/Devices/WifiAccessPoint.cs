using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.Primitives;

namespace LocalizationCore.SensorModel.Devices
{
    [Serializable]
    public class WifiAccessPoint : AccessPoint
    {
        public const double WIFI_BASE_RSSI = 30;

        public WifiAccessPoint(string name, Coord coord)
            : base(name, coord, WIFI_BASE_RSSI)
        {
        }

        
    }
}
