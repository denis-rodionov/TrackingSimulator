using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;
using LocalizationCore.BuildingModel;
using LogProvider;

namespace LocalizationCore.Algorithms
{
    class MarkovChain
    {
        const double INCREMENT = 0.1;
        const string DEBUG = "MarcovChain";

        List<ChainState> _states = new List<ChainState>();

        public TimeSpan ModelTime { get; set; }

        public MarkovChain()
        {
        }

        public ChainState findState(ChainItem item)
        {
            foreach (ChainState state in _states)
                if (state.Item == item)
                    return state;
            return null;
        }

        public void tuneState(ChainState currentState, ChainState lastState, Interval interval)
        {
            Transition tr = lastState.getTransitionToState(currentState);
            if (lastState.OutcomeTransitions.Count() > 1)
            {
                tr.Probability += INCREMENT;
                lastState.normalizeOutcomeTransitions();
            }
            currentState.addStatistics(interval);
            Logger.Log("State " + currentState + " tuned (" + ModelTime + ")");
        }

        public ChainState createState(ChainItem item, ChainState lastState, Interval interval)
        {
            ChainState newState = new ChainState(item, interval);
            Transition newTransition = new Transition(lastState, newState);
            _states.Add(newState);
            if (lastState != null)
            {
                lastState.addOutcomTransition(newTransition);
                newState.addIncomTransition(newTransition);
            }
            Logger.Log("State " + newState + " created (" + ModelTime + ")");
            return newState;
        }

        public ChainState getRouteDestination(Route route)
        {
            ChainState state = _states.Where(s => (Route)s.Item == route).Single();
            return state.OutcomeTransitions.Single().SecondState;
        }
    }
}
