using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Primitives
{
    class Line : ILine
    {
        public Coord A { get; set; }

        public Coord B { get; set; }
    }
}
