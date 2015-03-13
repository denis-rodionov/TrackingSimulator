using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.Algorithms
{
    public class AreaLocation : ChainItem
    {
        //static int count = 0;
        private string p;

        public SRect Area { get; set; }
        public string Name { get; set; }

        public AreaLocation(SRect area, string name)
        {
            Area = area;
            Name = name;
        }

        public static bool operator ==(AreaLocation loc1, AreaLocation loc2)
        {
            if ((object)loc1 == null || (object)loc2 == null)
                return PointerComparer.comparePointers(loc1, loc2);

            return loc1.Name == loc2.Name;
        }

        public static bool operator !=(AreaLocation loc1, AreaLocation loc2)
        {
            return !(loc1 == loc2);
        }

        public override string ToString()
        {
            return "{" + Name + "}";
        }
    }
}
