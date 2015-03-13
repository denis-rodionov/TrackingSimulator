using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.Primitives;

namespace LocalizationCore.SensorModel.Devices
{
    [Serializable]
    public abstract class AccessPoint : Device
    {
        /// <summary>
        /// Base signal strength of access point (1 meter from access point)
        /// </summary>
        public double SignalStrength { get; set; }

        public AccessPoint(string name, Coord coord, double ss)
            : base(name, coord)
        {
            this.SignalStrength = ss;
            this.Name = name;
            this.Coordinate = coord;
        }
    }
}
