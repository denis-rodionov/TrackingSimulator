using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.PersonModel
{
    public class ActionResult
    {
        public double Hunger { get; private set; }
        public double Thrist { get; private set; }
        public double BadMood { get; private set; }
        public double Toilet { get; private set; }
        public double Tiredness { get; private set; }

        public ActionResult(double hunger, double thrist, double mood, double toilet, double tiredness)
        {
            Hunger = hunger;
            Thrist = thrist;
            BadMood = mood;
            Toilet = toilet;
            Tiredness = tiredness;
        }
    }
}
