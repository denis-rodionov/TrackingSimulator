using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.ErrorEstimation
{
    public abstract class BaseErrorMapBuilder
    {
        /// <summary>
        /// Fill acuracy parameter for every map's location
        /// </summary>
        /// <param name="map"></param>
        public abstract void BuildAccuracyMap();
    }
}
