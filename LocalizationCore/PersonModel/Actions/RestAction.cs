using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.PersonModel.Actions
{
    class RestAction : PersonAction
    {
        const double DURATION = 2;  // in hours

        public RestAction(Room homeRoom, Characteristics character, Person person)
            : base(character, person)
        {
            ActionLocation = homeRoom;
            Duration = (int)(DURATION * 60 * 60);
            Name = "Rest";
            Result = new ActionResult(0, 0, 0, 0, 0.5);
        }

        protected override void OnTime(double seconds, PersonState state)
        {
            state.Tiredness -= seconds * Result.Tiredness / Duration * _character.RestNeeds;
        }

        public override PersonAction clone()
        {
            return new RestAction(ActionLocation, _character, Person);
        }
    }
}
