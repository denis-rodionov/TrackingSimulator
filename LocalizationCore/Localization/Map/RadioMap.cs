using LocalizationCore.SensorModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization
{
    public class RadioMap : IEnumerable<Observations>
    {
        Dictionary<Location, Observations> Observations { get; set; }

        public RadioMap()
        {
            Observations = new Dictionary<Location, Observations>();
        }

        public void RemoveObservations(Observations observations)
        {
            Observations.Remove(observations.LocationLink);
        }

        public IEnumerator<Observations> GetEnumerator()
        {
            return Observations.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Observations.Values.GetEnumerator();
        }

        public void Add(Location location, Fingerprint fingerprint)
        {
            if (Observations.Keys.Contains(location))
                Observations[location].AddCalibrationPoint(fingerprint);
            else
            {
                var ob = new Observations(location);
                ob.AddCalibrationPoint(fingerprint);
                Observations.Add(location, new Observations(location));
            }
        }

        public void Add(Location location, Observations observations)
        {
            Observations.Add(location, observations);
        }

        public void Add(RadioMap radioMap)
        {
            foreach (var item in radioMap)
                Observations.Add(item.LocationLink, item);
        }

        public Observations this[Location location]
        {
            get
            {
                return Observations[location];
            }
        }
    }
}
