using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;
using LocalizationCore.PersonModel.Actions;

namespace LocalizationCore.PersonModel
{
    public class PersonState
    {
        const double THRESOLD = 0.1;
        const double NIGHT_COEF = 0.1;
        const string SPECIFIC = "F";

        //public Coord Coordinate { set; get; }

        double _hunger;
        double _thirst; 
        double _toilet;
        double _tiredness;
        double _badMood;

        public PersonState()
        {
            //Coordinate = coord;
            Hunger = 0.5;
            Thirst = 0.5;
            Toilet = 0.5;
            Tiredness = 0.5;
            BadMood = 0.5;
        }

        public void onTime(double seconds, Characteristics character, PersonAction action)
        {
            double coef = 1;
            if (action is SleepAction)
                coef = NIGHT_COEF;

            if (action.Result.Hunger < THRESOLD)
                Hunger += character.hungerGrow(seconds) * coef;

            if (action.Result.Thrist < THRESOLD)
                Thirst += character.thirstGrow(seconds) * coef;

            if (action.Result.Toilet < THRESOLD)
                Toilet += character.toiletGrow(seconds) * coef;

            if (action.Result.Tiredness < THRESOLD)
                Tiredness += character.tirednessGrow(seconds) * coef;

            if (action.Result.BadMood < THRESOLD)
                BadMood += character.badMoodGrow(seconds) * coef;
        }

        public override string ToString()
        {
            return "{ H=" + Hunger.ToString(SPECIFIC) + ",Tr=" + Thirst.ToString(SPECIFIC) + ",Tl=" + 
                Toilet.ToString(SPECIFIC) + ",Td=" + Tiredness.ToString(SPECIFIC) + ",M=" + BadMood.ToString(SPECIFIC) + " }";
        }

        #region Properties

        public double Hunger {
            get { return _hunger; }
            set
            {
                _hunger = value;
                if (_hunger > 1)
                    _hunger = 1;
                else if (_hunger < 0)
                    _hunger = 0;
            }
        }


        public double Thirst 
        {
            get { return _thirst; }
            set
            {
                _thirst = value;
                if (_thirst > 1)
                    _thirst = 1;
                else if (_thirst < 0)
                    _thirst = 0;
            }
        }

        public double Toilet
        {
            get { return _toilet; }
            set
            {
                _toilet = value;
                if (_toilet > 1)
                    _toilet = 1;
                else if (_toilet < 0)
                    _toilet = 0;
            }
        }

        public double Tiredness
        {
            get { return _tiredness; }
            set
            {
                _tiredness = value;
                if (_tiredness > 1)
                    _tiredness = 1;
                else if (_tiredness < 0)
                    _tiredness = 0;
            }
        }

        public double BadMood
        {
            get { return _badMood; }
            set
            {
                _badMood = value;
                if (_badMood > 1)
                    _badMood = 1;
                else if (_badMood < 0)
                    _badMood = 0;
            }
        }

        #endregion Properties
    }
}
