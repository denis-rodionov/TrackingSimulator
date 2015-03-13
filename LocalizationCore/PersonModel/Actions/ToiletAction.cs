using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.PersonModel.Actions
{
    class ToiletAction : PersonAction
    {
        const int AVERAGE_DURATION = 5; // minutes

        public ToiletAction(Room toiletRoom, Characteristics character, Person person)
            : base(character, person)
        {
            ActionLocation = toiletRoom;
            Duration = AVERAGE_DURATION * 60;
            Name = "WC";
            Result = new ActionResult(0, 0, 0, 1, 0);
        }

        protected override void OnTime(double seconds, PersonState state)
        {
            state.Toilet -= seconds * Result.Toilet / Duration;
        }

        public override PersonAction clone()
        {
            return new ToiletAction(ActionLocation, _character, Person);
        }
    }

}
