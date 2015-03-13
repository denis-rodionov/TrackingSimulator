using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;
using LocalizationCore.PersonModel.Actions;
using LocalizationCore.PersonModel.Behaviours;
using LocalizationCore.Behaviours.PersonModel;

namespace LocalizationCore.PersonModel
{
    class PersonFactory
    {
        public static Person createVasya(Room home, FloorKnowledge floor)
        {
            Characteristics character = new Characteristics(0.3, 1, 1, 1);
            Person res = new Person(home, createSchedule(home, floor, character), floor, "Vasya", character);
            res.Behaviour = new WanderBehaviour(res.Character, res);    // гуляка
            var schedule = createSchedule(home, floor, character);
            //res.Behaviour = new Behaviour(schedule, floor, home, character, res.State, res);
            
            return res;
        }

        private static Schedule createSchedule(Room home, FloorKnowledge floor, Characteristics character)
        {
            Schedule res = new Schedule();

            ToiletAction morningToilet = new ToiletAction(floor.ToiletRoom, character, null);
            ScheduleItem morningToiletItem = new ScheduleItem(morningToilet, new TimeSpan(7, 30, 0), new TimeSpan(7, 45, 0));
            res.addItem(morningToiletItem);

            MealAction breakfast = new MealAction("Breakfast", floor.MealRoom, character, null);
            ScheduleItem breakfastItem = new ScheduleItem(breakfast, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0));
            res.addItem(breakfastItem);

            MealAction lunch = new MealAction("Lunch", floor.MealRoom, character, null);
            ScheduleItem lunchItem = new ScheduleItem(lunch, new TimeSpan(13, 0, 0), new TimeSpan(14, 0, 0));
            res.addItem(lunchItem);

            MealAction dinner = new MealAction("Dinner", floor.MealRoom, character, null);
            ScheduleItem dinnerItem = new ScheduleItem(dinner, new TimeSpan(19, 0, 0), new TimeSpan(20, 30, 0));
            res.addItem(dinnerItem);

            SleepAction sleep = new SleepAction(home, character, null);
            ScheduleItem sleepItem = new ScheduleItem(sleep, new TimeSpan(21, 0, 0), new TimeSpan(23, 0, 0));
            res.addItem(sleepItem);

            SleepAction sleep2 = new SleepAction(home, character, null);
            ScheduleItem sleepItem2 = new ScheduleItem(sleep2, new TimeSpan(0, 0, 0), new TimeSpan(8, 0, 0));
            res.addItem(sleepItem2);

            return res;
        }
    }
}
