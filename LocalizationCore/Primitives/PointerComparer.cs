using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.Primitives
{
    class PointerComparer
    {
        public static bool comparePointers(object coord1, object coord2)
        {
            if (coord1 == coord2)
                return true;
            else
                return false;
        }
    }
}
