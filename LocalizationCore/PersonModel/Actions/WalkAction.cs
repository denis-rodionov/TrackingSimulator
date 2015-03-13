using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.BuildingModel;

namespace LocalizationCore.PersonModel.Actions
{
    class WalkAction : PersonAction
    {
        const int WALK_DURATION = 8;        // in hours

        public WalkAction(Characteristics character, Person person)
            : base(character, person)
        {
            ActionLocation = null;
            Duration = WALK_DURATION * 60 * 60;
            Name = "Walking";
            Result = new ActionResult(0, 0, 1, 0, 0);
        }

        protected override void OnTime(double seconds, PersonState state)
        {
            if (_router.isFinished())
                throw new FinishedActionException();
            MakeMove(seconds, state);
        }

        public override void Start(PersonState state)
        {
            State = ActionState.MAIN_ACTION;
            _router.CreateDefaultRoute();
            //_router.IntelectualRoutes();
        }

        public override PersonAction clone()
        {
            return new WalkAction(_character, Person);
        }
    }
}
