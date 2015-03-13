using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Primitives
{
    public interface ILine
    {
        Coord A { get; }
        Coord B { get; }
    }
}
