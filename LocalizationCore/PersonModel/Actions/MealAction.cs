using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.PersonModel.Actions
{
    class MealAction : PersonAction
    {
        const int MEAL_DURATION = 60;   // in minutes

        public MealAction(string name, Room location, Characteristics character, Person person)
            : base(character, person)
        {
            Name = name;
            Duration = MEAL_DURATION * 60;  // in secconds
            Result = new ActionResult(0.8, 0.5, 0.3, 0, 0.2);
            ActionLocation = location;
        }

        protected override void OnTime(double seconds, PersonState state)
        {
            state.Hunger -= seconds * Result.Hunger / Duration / _character.EatNeeds;
            state.Thirst -= seconds * Result.Thrist / Duration;
            state.Tiredness -= seconds * Result.Tiredness / Duration / _character.RestNeeds;
            state.BadMood -= seconds * Result.BadMood / Duration / _character.CommunicationNeeds;
        }

        public override PersonAction clone()
        {
            return new MealAction(Name, ActionLocation, _character, Person);
        }
    }
}
