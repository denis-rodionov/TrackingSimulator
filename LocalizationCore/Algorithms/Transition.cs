using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.Algorithms
{
    class Transition
    {
        public ChainState FirstState { get; set; }
        public ChainState SecondState { get; set; }
        public double Probability { get; set; }

        public Transition(ChainState lastState, ChainState newState)
        {
            FirstState = lastState;
            SecondState = newState;
            Probability = 1;
        }
    }
}
