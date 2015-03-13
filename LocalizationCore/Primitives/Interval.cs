using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalizationCore.Primitives
{
    class Interval
    {
        public TimeSpan BeginTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public Interval(TimeSpan beginTime, TimeSpan endTime)
        {
            BeginTime = beginTime;
            EndTime = endTime;
        }

        public double getDuration()
        {
            return (double)(EndTime - BeginTime).Seconds;
        }
    }
}
