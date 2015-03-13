using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.BuildingModel;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel.Devices;

namespace LocalizationCore.SensorModel.Sensors
{
    public class HybridSensor : RssiSensor
    {
        WifiRssiSensor wifi = new WifiRssiSensor();
        RfidSensor rfid = new RfidSensor();

        public override IEnumerable<Device> GetAccessPoints()
        {
            return Building.Instance.Floor.Devices.ToList();
        }

        public override double RssiCalculation(AccessPoint ap, Coord coord)
        {
            //var fitting = SimpleRNG.GetNormal(0.1, 1);
            switch (ap.GetType().Name)
            {
                case "WifiAccessPoint":
                    return wifi.RssiCalculation(ap, coord);// + fitting;
                case "RfidReader":
                    return rfid.RssiCalculation(ap, coord);// + fitting;
                default:
                    throw new Exception("Uncknown access point!");
            }
        }
    }
}
