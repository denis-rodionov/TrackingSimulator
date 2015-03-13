using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;
using LocalizationCore.BuildingModel;
using LogProvider;
using LocalizationCore.SensorModel;
using LocalizationCore.Behaviours.PersonModel;
using LocalizationCore.SensorModel.Sensors;
using LocalizationCore.PersonModel.Actions;

namespace LocalizationCore.PersonModel
{
    public class Person
    {
        public Behaviour Behaviour { get; set; }
        PersonAction _currentAction = null;

        public string Name { private set; get; }
        public PersonState State { private set; get; }      // hunger, tiredness, location.... I love burgers
        public Room HomeRoom { set; get; }
        public Characteristics Character { private set; get; }    
    
        // Sensors
        //public RssiSensor WifiRssiSensor { get; set; }
        //public RfidSensor RfidSensor { get; set; }
        //public HybridSensor HybridSensor { get; set; }

        public event Action<Person, PersonAction> ActionChanged;
        public event Action<Person, PersonAction> ActionFinished;
        public event Action<Person, Coord, Coord> PersonMoved;
        

        /// <summary>
        /// Constructor
        /// </summary>
        public Person(Room homeRoom, Schedule schedule, FloorKnowledge floor, string name, Characteristics character)
        {
            Name = name;
            Character = character;
            Behaviour = new Behaviour(schedule, floor, homeRoom, Character, State, this);
            State = new PersonState();
            coordinate = new Coord(1.5, 1);

            //WifiRssiSensor = new WifiRssiSensor();
            //RfidSensor = new RfidSensor();
            //HybridSensor = new HybridSensor();
            
            HomeRoom = homeRoom;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Called when some period of time passed
        /// </summary>
        /// <param name="milliseconds"></param>
        public void onTime(double seconds, TimeSpan modelTime)
        {
            PersonAction nextAction = Behaviour.EvaluateState(State, CurrentAction, modelTime);        // action changing
            if (nextAction != null)
                CurrentAction = nextAction;
            _onTime(seconds, modelTime);
            State.onTime(seconds, Character, CurrentAction);
        }

        private void _onTime(double seconds, TimeSpan modelTime)
        {
            try
            {
                CurrentAction.onTime(seconds, State);
            }
            catch (FinishedActionException)
            {
                CurrentAction = null;
                CurrentAction = Behaviour.EvaluateState(State, CurrentAction, modelTime);
            }
        }

        public void CoordinateChanged(Coord c1, Coord c2)
        {
        }

        public PersonAction CurrentAction
        {
            get { return _currentAction; }
            set
            {
                if (_currentAction != null)
                {
                    _currentAction.stop();
                    if (ActionFinished != null)
                        ActionFinished(this, _currentAction);
                    Logger.Log(_currentAction + " stoped (" + Model.Instance.ModelTime + ")");
                }
                _currentAction = value;
                
                if (_currentAction != null)
                {
                    _currentAction.Person = this;
                    _currentAction.Start(State);
                    if (ActionChanged != null)
                        ActionChanged(this, _currentAction);
                    Logger.Log(_currentAction + " started (" + Model.Instance.ModelTime + "), State = " + State);
                }
            }
        }

        Coord coordinate;
        public Coord Coordinate
        {
            get
            {
                return coordinate;
            }
            set
            {
                var oldCoord = coordinate;
                coordinate = value;
                if (PersonMoved != null)
                    PersonMoved(this, oldCoord, coordinate);
            }
        }
    }
}
