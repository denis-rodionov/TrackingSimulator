using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;

namespace LocalizationCore.Algorithms
{
    class ProbabilityState
    {
        public ChainState State { get; set; }
        public double Probability { get; set; }
    }

    class ChainState
    {
        public ChainItem Item { get; set; }
        public List<Transition> IncomeTransitions { get; set; }
        public List<Transition> OutcomeTransitions { get; set; }

        private List<Interval> _intervals = new List<Interval>();      // statistics

        public ChainState(ChainItem item, Interval interval)
        {
            Item = item;
            IncomeTransitions = new List<Transition>();
            OutcomeTransitions = new List<Transition>();
            _intervals.Add(interval);
        }

        public void addOutcomTransition(Transition newTransition)
        {
            int count = OutcomeTransitions.Count();
            newTransition.Probability = 1 / (double)count;
            normalizeOutcomeTransitions();
        }

        public void normalizeOutcomeTransitions()
        {
            double sum = 0;
            foreach (Transition t in OutcomeTransitions)
                sum += t.Probability;

            foreach (Transition t in OutcomeTransitions)
                t.Probability = t.Probability / sum;
        }

        public void addIncomTransition(Transition newTransition)
        {
            IncomeTransitions.Add(newTransition);
        }

        public Transition getTransitionToState(ChainState currentState)
        {
            foreach (Transition t in OutcomeTransitions)
                if (t.SecondState == currentState)
                    return t;

            throw new Exception("No one transition leads to the state " + currentState + " from " + this);
        }

        public void addStatistics(Interval interval)
        {
            _intervals.Add(interval);
        }

        public override string ToString()
        {
            return Item.ToString();
        }

        /// <summary>
        /// Returns averrage time
        /// </summary>
        /// <returns>In seconds</returns>
        public double averageTime()
        {
            double res = 0;
            foreach (Interval interval in _intervals)
                res += interval.getDuration();
            return res;
        }

        public IEnumerable<ProbabilityState> nextStates()
        {
            List<ProbabilityState> result = new List<ProbabilityState>();
            foreach (Transition t in OutcomeTransitions)
            {
                ProbabilityState ps = new ProbabilityState();
                ps.Probability = t.Probability;
                ps.State = t.SecondState;
                result.Add(ps);
            }
            return result;
        }
    }
}
