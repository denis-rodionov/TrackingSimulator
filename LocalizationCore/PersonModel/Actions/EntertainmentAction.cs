using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.PersonModel.Actions
{
    class EntertainmentAction : PersonAction
    {
        const int DURATION = 2;  // in hours

        public EntertainmentAction(Room communicationRoom, Characteristics character, Person person)
            : base(character, person)
        {
            ActionLocation = communicationRoom;
            Duration = DURATION * 60 * 60;
            Name = "Communication";
            Result = new ActionResult(0, 0, 1, 0, 0);
        }

        protected override void OnTime(double seconds, PersonState state)
        {
            state.BadMood -= seconds * Result.BadMood / Duration / _character.CommunicationNeeds;
        }

        public override PersonAction clone()
        {
            return new EntertainmentAction(ActionLocation, _character, Person);
        }
    }
}
