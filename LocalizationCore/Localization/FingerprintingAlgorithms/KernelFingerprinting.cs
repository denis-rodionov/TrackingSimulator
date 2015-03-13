using LocalizationCore.Primitives;
using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.FingerprintingAlgorithms
{
    public class KernelFingerprinting : Fingerprinting
    {
        public KernelFingerprinting(string name, RssiSensor sensor)
            : base(name, sensor, CalculationAlgorithm.KMax)
        {
        }

        public override Pdf GetLikelihood(Fingerprint fingerprint)
        {
            throw new NotImplementedException();
        }

    }
}
