using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;

namespace LocalizationCore.Algorithms
{
    enum PhysicalState { Movement, Stay }

    class MoveDetector
    {
        const int QUEUE_SIZE = 5;
        const double ERROR_THRESOLD = 3;         // error in meters

        Queue<Coord> _queue = new Queue<Coord>();
        Queue<TimeSpan> _times = new Queue<TimeSpan>();


        public void newCoord(Coord coord, TimeSpan modelTime)
        {
            _queue.Enqueue(coord);
            _times.Enqueue(modelTime);
            if (_queue.Count() > QUEUE_SIZE)
            {
                _queue.Dequeue();
                _times.Dequeue();
            }
        }

        public PhysicalState getState()
        {
            if (_queue.Count() == 0)
                return PhysicalState.Stay;
            else
                return Coord.Distance(_queue.First(), _queue.Last()) > ERROR_THRESOLD ? PhysicalState.Movement : PhysicalState.Stay;
        }
    }
}
