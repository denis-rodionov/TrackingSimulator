using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;
using LocalizationCore.Primitives;

namespace LocalizationCore.PersonModel.Actions
{
    public enum ActionState { NOT_ACTIVE, GO_TO_LOCATION, MAIN_ACTION, FINISHED};

    public class FinishedActionException : Exception { };

    public abstract class PersonAction
    {
        protected Characteristics _character;
        //protected PersonState _state;
        protected double _remaining;
        protected int _duration;
        protected RouteManager _router = new RouteManager();

        //event Action<Person, Action> Finished;

        //--------------------------------------------------
        // public parameters
        public string Name { get; set; }

        public ActionState State { get; set; }

        /// <summary>
        /// Determine which parameters will grow and how much
        /// </summary>
        public ActionResult Result { get; set; }

        /// <summary>
        /// Where action will be take place
        /// </summary>
        public Room ActionLocation { get; set; }

        public Person Person { get; set; }
        
        /// <summary>
        /// Constructer
        /// </summary>
        /// <param name="character">Person parameters</param>
        public PersonAction(Characteristics character, Person person)
        {
            _character = character;
            //_state = state;
            State = ActionState.NOT_ACTIVE;
            Person = person;
        }

        /// <summary>
        /// Duration of the action in seconds
        /// </summary>
        public int Duration
        {
            get { return _duration; }
            set 
            {
                _remaining = value;
                _duration = value;
            }
        }

        /// <summary>
        /// Simulation of an action.
        /// </summary>
        /// <param name="seconds"></param>
        public void onTime(double seconds, PersonState state)
        {
            if (State == ActionState.NOT_ACTIVE)
                throw new Exception("Cannot handle time event in NON_ACTIVE state");
            else if (State == ActionState.GO_TO_LOCATION)
                MakeMove(seconds, state);
            else if (State == ActionState.MAIN_ACTION)
            {
                if (_remaining < 0)
                {
                    State = ActionState.FINISHED;
                    throw new FinishedActionException();
                }
                OnTime(seconds, state);  // abstract function
                _remaining -= seconds;
            }
        }

        #region MoveAction

        public void MakeMove(double seconds, PersonState state)
        {
            double move = _character.getMove(seconds);
            while (move > 0)
            {
                if (_router.isFinished())
                {
                    State = ActionState.MAIN_ACTION;
                    break;
                }
                Coord nextTarget = _router.getNextTarget();
                _makeMove(ref move, nextTarget, state);
            }
        }

        private void _makeMove(ref double move, Coord nextTarget, PersonState state)
        {
            SVector dir = new SVector(Person.Coordinate, nextTarget);
            if (dir.Length < move)
            {
                move -= dir.Length;
                Person.Coordinate = nextTarget;
                _router.deletePoint();
            }
            else
            {
                dir = dir * (move / dir.Length);
                Person.Coordinate = Person.Coordinate + dir;
                move = 0;
            }
        }

        #endregion MoveAction

        /// <summary>
        /// for derivative classes
        /// </summary>
        protected abstract void OnTime(double seconds, PersonState state);

        /// <summary>
        /// The owner person began this action
        /// </summary>
        public virtual void Start(PersonState state)
        {
            if (ActionLocation == null)
                State = ActionState.MAIN_ACTION;
            else
            {
                State = ActionState.GO_TO_LOCATION;
                //throw new NotImplementedException("Need to implement route manager!");
            }
        }

        /// <summary>
        /// The owner person stoped this action
        /// </summary>
        public void stop()
        {
            //State = ActionState.FINISHED;
        }

        public abstract PersonAction clone();

        public virtual void reset()
        {
            _remaining = 0;
            State = ActionState.NOT_ACTIVE;
        }

        public override string ToString()
        {
            string pre = State == ActionState.GO_TO_LOCATION ? "" : "";
            return pre + Name;
        }

        public double doneProgress()
        {
            return (Duration - _remaining) / (double)Duration;
        }
    }
}
