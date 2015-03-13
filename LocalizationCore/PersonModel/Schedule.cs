using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.PersonModel.Actions;

namespace LocalizationCore.PersonModel
{
    public class ScheduleItem
    {
        public PersonAction Action { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Finish { get; set; }
        public bool Done { get; set; }

        public ScheduleItem(PersonAction action, TimeSpan start, TimeSpan finish)
        {
            Action = action;
            Start = start;
            Finish = finish;
            Done = false;
        }
    }

    class OvelapedActionsException : Exception
    {
        public IEnumerable<ScheduleItem> ScheduleItems { get; private set; }

        public OvelapedActionsException(IEnumerable<ScheduleItem> items)
        {
            ScheduleItems = items;
        }
    }

    public class Schedule
    {
        List<ScheduleItem> _list = new List<ScheduleItem>();
        int _passedDays = 0;

        public Schedule()
        {
        }

        public PersonAction getNowAction(TimeSpan time)
        {
            IEnumerable<ScheduleItem> res = getItems(ref time);
            if (res.Count() == 1)
                return res.Single().Action;
            else if (res.Count() > 1)
                throw new OvelapedActionsException(res);
            else
                throw new Exception("No scheduled actions for now!");
        }

        private IEnumerable<ScheduleItem> getItems(ref TimeSpan time)
        {
            TimeSpan dayTime = time;
            dayTime = dayTime.Subtract(new TimeSpan(time.Days, 0, 0, 0));       // substruct days component
            IEnumerable<ScheduleItem> res = _list.Where(s => dayTime > s.Start && dayTime < s.Finish 
                && s.Action.State != ActionState.FINISHED);
            return res;
        }

        /// <summary>
        /// Check if there is actions in the schedule
        /// </summary>
        /// <param name="time">time for check</param>
        /// <returns></returns>
        public bool hasNowAction(TimeSpan time)
        {
            //checkForNewDay(time);
            IEnumerable<ScheduleItem> res = getItems(ref time);
            return res.Count() != 0;
        }

        private void checkForNewDay(TimeSpan time)
        {
            if (time.Days > _passedDays)
                cleanAllActions();
        }

        private void cleanAllActions()
        {
            foreach (ScheduleItem si in _list)
                si.Action.reset();
        }

        public void addItem(ScheduleItem item)
        {
            _list.Add(item);
        }
    }
}
