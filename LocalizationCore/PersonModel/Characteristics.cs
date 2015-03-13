using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.PersonModel
{
    public class Characteristics
    {
        // in seconds
        const double HUNGER_INC = 5.5e-5;
        const double THIRST_INC = 9.25e-5;
        const double TIREDNESS_INC = 1.16e-5;
        const double MOOD_INC = 4.63e-5;
        const double TOILET_INC = 13.9e-5;

        public double MoveInSecond { get; set; }
        public double CommunicationNeeds { get; set; }
        public double EatNeeds { get; set; }
        public double RestNeeds { get; set; }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="moveLength">Move (meters) in one second</param>
        /// <param name="communicationNeed">1 - normal, >1 - increased needs...</param>
        public Characteristics(double moveLength, double communicationNeed, double eatNeeds, double restNeeds)
        {
            MoveInSecond = moveLength;
            CommunicationNeeds = communicationNeed;
            EatNeeds = eatNeeds;
            RestNeeds = restNeeds;
        }

        public double getMove(double seconds)
        {
            return MoveInSecond * seconds;
        }

        public double hungerGrow(double seconds)
        {
            return seconds * HUNGER_INC * EatNeeds;
        }

        public double thirstGrow(double seconds)
        {
            return seconds * THIRST_INC;
        }

        public double toiletGrow(double seconds)
        {
            return seconds * TOILET_INC;
        }

        public double tirednessGrow(double seconds)
        {
            return seconds * TIREDNESS_INC * RestNeeds;
        }

        public double badMoodGrow(double seconds)
        {
            return seconds * MOOD_INC * CommunicationNeeds;
        }
    }
}
