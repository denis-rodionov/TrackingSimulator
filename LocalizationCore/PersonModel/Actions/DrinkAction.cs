using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.PersonModel.Actions
{
    /// <summary>
    /// thrust slaking
    /// </summary>
    class DrinkAction : PersonAction
    {
        const int DURATION = 60;             // in seconds

        public DrinkAction(Room drinkRoom, Characteristics character, Person person)
            : base(character, person)
        {
            ActionLocation = drinkRoom;
            Duration = DURATION;
            Result = new ActionResult(0, 1, 0, 0, 0);
            Name = "Drinking";
        }

        protected override void OnTime(double seconds, PersonState state)
        {
            state.Thirst -= seconds * Result.Thrist / Duration;
        }

        public override PersonAction clone()
        {
            return new DrinkAction(ActionLocation, _character, Person);
        }
    }
}
