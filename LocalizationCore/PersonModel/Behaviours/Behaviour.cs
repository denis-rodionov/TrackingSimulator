using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.PersonModel.Actions;
using LocalizationCore.BuildingModel;
using LocalizationCore.PersonModel;

namespace LocalizationCore.Behaviours.PersonModel
{
    public class Behaviour
    {
        const double LOW_LIMIT = 0.2;
        const double DANGER_LIMIT = 0.1;
        const double DONE_COEFFICIENT = 10;     // more important to finish started action
        const double SHORT_ACTION_TIME = 1800;  // max short action time
        const double COMMON_THRESOLD = 0.75;

        Schedule _schedule;                             // actions schedule
        List<PersonAction> _unscheduled;                // perform when need
        Characteristics _character;                     // person parameters
        Person _person;                                  // person reference

        public Behaviour(Schedule schedule, FloorKnowledge floor, Room homeRoom, 
                        Characteristics character, PersonState state, Person person)
        {
            _person = person;
            _character = character;
            _schedule = schedule;
            _unscheduled = new List<PersonAction>();
            _unscheduled.Add(new SleepAction(homeRoom, character, person));
            _unscheduled.Add(new RestAction(homeRoom, character, person));
            _unscheduled.Add(new DrinkAction(floor.MealRoom, character, person));
            _unscheduled.Add(new ToiletAction(floor.ToiletRoom, character, person));
            _unscheduled.Add(new EntertainmentAction(floor.EntertainmentRoom, character, person));
        }

        /// <summary>
        /// Checks if current action must be changed by another one 
        /// in order to sutisfy needss
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentAction"></param>
        /// <param name="modelTime"></param>
        /// <returns></returns>
        public virtual PersonAction EvaluateState(PersonState state, PersonAction currentAction, TimeSpan modelTime)
        {
            PersonAction nextAction = null;
            nextAction = checkScheduled(state, modelTime);  // interupt if exists

            if (nextAction == null)
                nextAction = checkUnscheduled(state, currentAction);
            
            if (currentAction != null && nextAction != null && 
                nextAction.GetType() == currentAction.GetType())  // if the same action
                        nextAction = null;      // do not interupt

            checkRules(ref nextAction, currentAction, state);

            return nextAction;
        }

        /// <summary>
        /// Checking rules
        /// </summary>
        /// <param name="action"></param>
        private void checkRules(ref PersonAction next, PersonAction current, PersonState state)
        {
            if (current != null)
            {
                if (current.Duration < SHORT_ACTION_TIME && current.doneProgress() < 0.9)
                    next = null;    // not interupt
                else if (current is ToiletAction && state.Toilet < COMMON_THRESOLD)
                    next = null;
                else if (current is DrinkAction && state.Thirst < COMMON_THRESOLD)
                    next = null;
            }

            if (current == null && next == null)
                next = getIdleAction();
        }

        private PersonAction getIdleAction()
        {
            return _unscheduled.Where(a => a is RestAction).Single().clone();
        }

        private PersonAction checkUnscheduled(PersonState state, PersonAction currentAction)
        {
            PersonAction res = null;

            PersonAction bestAction = getTheBest(_unscheduled, state);
            double bestImportance = estimateAction(bestAction, state);

            if (currentAction != null)
            {
                double currentImportance = estimateAction(currentAction, state);
                if (bestImportance > currentImportance * DONE_COEFFICIENT)
                    res = bestAction.clone();
            }
            else
                res = bestAction;

            checkThresoldRules(ref res, state);

            return res;
        }

        private void checkThresoldRules(ref PersonAction action, PersonState state)
        {
            if (action is ToiletAction && state.Toilet < COMMON_THRESOLD)
                action = null;
            if (action is DrinkAction && state.Thirst < COMMON_THRESOLD)
                action = null;
            if (action is SleepAction && state.Tiredness < COMMON_THRESOLD)
                action = null;
        }

        private PersonAction checkScheduled(PersonState state, TimeSpan modelTime)
        {
            PersonAction nextAction = null;
            try
            {
                if (_schedule.hasNowAction(modelTime))
                    nextAction = _schedule.getNowAction(modelTime);
            }
            catch (OvelapedActionsException ex)
            {
                IEnumerable<PersonAction> res = ex.ScheduleItems.Select(s => s.Action);
                nextAction = getTheBest(res, state);
            }
            return nextAction;
        }

        /// <summary>
        /// Gets the most important action in the list
        /// </summary>
        /// <param name="res"></param>
        private PersonAction getTheBest(IEnumerable<PersonAction> list, PersonState state)
        {
            PersonAction best = null;
            double bestEstimation = 0;
            foreach (PersonAction act in list)
            {
                double est = estimateAction(act, state);
                if (est > bestEstimation)
                {
                    bestEstimation = est;
                    best = act;
                }
            }

            return best;
        }

        private double estimateAction(PersonAction act, PersonState state)
        {
            double sum = 0;
            sum += estimateParameter(state.Hunger, act.Result.Hunger, 0.3);
            sum += estimateParameter(state.Thirst, act.Result.Thrist, 0.5);
            sum += estimateParameter(state.Tiredness, act.Result.Tiredness, 0);
            sum += estimateParameter(state.Toilet, act.Result.Toilet, 0.7);
            sum += estimateParameter(state.BadMood, act.Result.BadMood, 0.4);

            return sum;
        }

        /// <summary>
        /// Estimate, how important to improove this parameter by specific value
        /// </summary>
        /// <param name="state">parameter to improove</param>
        /// <returns></returns>
        private double estimateParameter(double state, double profit, double thresold)
        {
            double res;
            if (profit > state)
                res = state;
            else
                res = profit;

            if (1 - state < DANGER_LIMIT)
                res *= 4;
            if (1 - state < LOW_LIMIT)
                res *= 2;
            return res;
        }

        
    }
}
