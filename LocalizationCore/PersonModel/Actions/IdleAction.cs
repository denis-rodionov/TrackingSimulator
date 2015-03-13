using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.PersonModel.Actions
{
    class IdleAction : PersonAction
    {
        public IdleAction(string name, Room location, Characteristics character, Person person)
            : base(character, person)
        {
            Name = name;
            Duration = int.MaxValue;
            Result = new ActionResult(0, 0, 0, 0, 0);
            ActionLocation = location;
        }

        protected override void OnTime(double seconds, PersonState state)
        {
        }

        public override PersonAction clone()
        {
            return new IdleAction(Name, ActionLocation, _character, Person);
        }
    }
}
