using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.Behaviours.PersonModel;
using LocalizationCore.BuildingModel;
using LocalizationCore.PersonModel.Actions;

namespace LocalizationCore.PersonModel.Behaviours
{
    class WanderBehaviour : Behaviour
    {
        Person Person { get; set; }
        Characteristics Character { get; set; }
        bool Done { get; set; }

        public WanderBehaviour(Characteristics character, Person person)
            : base(null, Building.Instance.Floor.GetFloorKnowledge(), null, character, null, person)
        {
            Person = person;
            Character = character;
            Done = false;
        }

        public override PersonAction EvaluateState(PersonState state, PersonAction currentAction, TimeSpan modelTime)
        {
            if (currentAction == null && !Done)
            {
                Done = true;
                return new WalkAction(Character, Person);
            }
            else if (currentAction == null)
                return new IdleAction("Idle", null, Character, Person);
            else
                return null;    // do not interupt
        }
    }
}
