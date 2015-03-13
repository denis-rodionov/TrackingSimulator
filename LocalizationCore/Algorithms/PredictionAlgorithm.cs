using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogProvider;
using LocalizationCore.Primitives;

namespace LocalizationCore.Algorithms
{
    

    class PredictionAlgorithm
    {
        const string DEBUG = "PredictionAlgorithm";

        public string PersonName { get; set; }

        private MarkovChain Chain { get; set; }
        private MoveDetector MoveDetector { get; set; }
        private LastCoords LastCoords { get; set; }
        private ChainState LastState { get; set; }
        private TimeSpan CurrentStateBegin { get; set; }

        public PredictionAlgorithm(string personName)
        {
            PersonName = personName;
            Chain = new MarkovChain();
            MoveDetector = new MoveDetector();
            LastCoords = new LastCoords();
        }

        public PredictionResult predict(Coord currentCoord, double seconds)
        {
            return _predict(LastState, seconds, LastCoords.getInterval().getDuration(), 100);
        }

        /// <summary>
        /// Recursive prediction algorithm.
        /// </summary>
        /// <param name="lastState">The state we predict from</param>
        /// <param name="predictionTime">Time we want to predict ahead</param>
        /// <param name="lastStateTime">Time passed from the state begin</param>
        /// <returns>Collection of locations with probabilities</returns>
        private PredictionResult _predict(ChainState lastState, double predictionTime, double lastStateTime, double confidence)
        {
            Logger.Log("_predict(lastState=" + lastState + ", predictionTime=" + predictionTime +
                            ", lastStateTime=" + lastStateTime + ", confidence=" + confidence + ")");

            PredictionResult res = new PredictionResult();            
            double exitTime = lastState.averageTime() - lastStateTime;

            if (lastState.Item is Route)
            {
                Route route = (Route)lastState.Item;
                if (predictionTime > exitTime)
                    res.addResult(_predict(Chain.getRouteDestination(route), predictionTime - exitTime, 0, confidence));
                else
                    res.add(route.getRouteLocations(), confidence);
            }
            else if (lastState.Item is AreaLocation)
            {
                AreaLocation location = (AreaLocation)lastState.Item;
                
                IEnumerable<ProbabilityState> nextStates = lastState.nextStates();
                if (predictionTime > exitTime)
                    foreach (ProbabilityState state in nextStates)
                        res.addResult(_predict(state.State, predictionTime - exitTime, 0, state.Probability));
                else
                    res.add(location, confidence);
            }

            Logger.Log("_predict returned: " + res);

            return res;
        }

        /// <summary>
        /// For statistics collection.
        /// Adds necessary states and corrects transitions.
        /// 1. Known route -> exit
        /// 2. Unknown route -> building new route
        /// 3. Known location -> exit
        /// 4. Unknown location -> building new location
        /// 5. Movement started -> change location state statistics
        /// 6. Movement ended -> change route state statistics
        /// </summary>
        public void onNewCoord(Coord coord, TimeSpan modelTime)
        {
            Chain.ModelTime = modelTime;        // for debug
            PhysicalState lastPhyState = MoveDetector.getState();
            MoveDetector.newCoord(coord, modelTime);
            PhysicalState nowState = MoveDetector.getState();

            if (lastPhyState == PhysicalState.Stay && nowState == PhysicalState.Stay)
                continueStaying(coord, modelTime);
            if (lastPhyState == PhysicalState.Movement && nowState == PhysicalState.Movement) 
                continueMovement(coord, modelTime);
            if (lastPhyState == PhysicalState.Stay && nowState == PhysicalState.Movement)  
                startMovementEvent(coord, modelTime);
            if (lastPhyState == PhysicalState.Movement && nowState == PhysicalState.Stay)
                finishMovementEvent(coord, modelTime);
        }

        private void continueStaying(Coord coord, TimeSpan modelTime)
        {
            LastCoords.addCoord(coord, modelTime);
        }

        private void continueMovement(Coord coord, TimeSpan modelTime)
        {
            LastCoords.addCoord(coord, modelTime);
        }

        private void finishMovementEvent(Coord coord, TimeSpan modelTime)
        {
            Route item = LastCoords.createRoute();
            _stateChanged(coord, modelTime, item);
        }

        private void startMovementEvent(Coord coord, TimeSpan modelTime)
        {
            AreaLocation item = LastCoords.createLocation();
            LastCoords.addCoord(LastCoords.LastCoord, modelTime);   // to strart route from the first location
            LastCoords.addCoord(coord, modelTime);
            _stateChanged(coord, modelTime, item);
        }

        private TimeSpan _stateChanged(Coord coord, TimeSpan modelTime, ChainItem item)
        {
            ChainState state = Chain.findState(item);
            if (state != null)      // chain contains item
            {
                if (LastState != null)
                    Chain.tuneState(state, LastState, LastCoords.getInterval());
                LastState = state;
            }
            else
                LastState = Chain.createState(item, LastState, LastCoords.getInterval());

            LastCoords.reset();
            LastCoords.addCoord(coord, modelTime);
            return modelTime;
        }
        
    }
}
