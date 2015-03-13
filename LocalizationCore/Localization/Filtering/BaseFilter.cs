

using LocalizationCore.Localization.FingerprintingAlgorithms;
using LocalizationCore.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.Filtering
{
    public abstract class BaseFilter : LocalizationAlgorithm
    {
        public LocalizationAlgorithm UnderlyingAlgorithm { get; private set; }

        public BaseFilter(string name, LocalizationAlgorithm underlyingAlgorithm) : base(name)
        {
            UnderlyingAlgorithm = underlyingAlgorithm;
        }

        public override Coord GetPriorError(Coord coord, AccuracyAlgorithm alg)
        {
            return UnderlyingAlgorithm.GetPriorError(coord, alg);
        }
    }
}
