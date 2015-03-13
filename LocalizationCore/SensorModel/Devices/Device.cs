using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.Primitives;

namespace LocalizationCore.SensorModel.Devices
{
    [Serializable]
    public abstract class Device
    {
        public string Name { get; set; }
        public Coord Coordinate { get; set; }

        public Device(string name, Coord coord)
        {
            Name = name;
            Coordinate = coord;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return this.GetHashCode() == ((Device)obj).GetHashCode();
        }

        public override int GetHashCode()
        {
            var name = Name == null ? "null" : Name;
            var coord = Coordinate == null ? "null" : Coordinate.ToString();
            return (name + coord).GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
