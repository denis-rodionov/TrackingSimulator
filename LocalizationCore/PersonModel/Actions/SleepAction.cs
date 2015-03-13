using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.PersonModel.Actions
{
    class SleepAction : PersonAction
    {
        const double SLEEP_DURATION = 8;        // in hours

        public SleepAction(Room homeRoom, Characteristics character, Person person)
            : base(character, person)
        {
            ActionLocation = homeRoom;
            Duration = (int)(SLEEP_DURATION * 60 * 60);
            Name = "Sleep";
            Result = new ActionResult(0, 0, 0.5, 0, 1);
        }

        protected override void OnTime(double seconds, PersonState state)
        {
            state.Tiredness -= seconds * Result.Tiredness / Duration / _character.RestNeeds;
            state.BadMood -= seconds * Result.Tiredness / Duration / _character.RestNeeds;
        }

        public override PersonAction clone()
        {
            return new SleepAction(ActionLocation, _character, Person);
        }
    }
}
