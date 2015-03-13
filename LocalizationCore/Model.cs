using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;
using LocalizationCore.Algorithms;
using LocalizationCore.Primitives;
using LogProvider;
using LocalizationCore.PersonModel;
using LocalizationCore.PersonModel.Actions;

namespace LocalizationCore
{
    public class Model
    {
        //const double SECONDS_PER_TIME = 1;
        TimeSpan TIME_PER_STEP = new TimeSpan(0, 0, 0, 0, 200);

        static Model _inst;

        public List<Person> Patients { get; set; }
        public TimeSpan ModelTime { get; set; }

        public event Action<string> ModelEventOccurred;

        public static Model Instance
        {
            get
            {
                if (_inst == null)
                    _inst = new Model();
                return _inst;
            }
        }

        private Model()
        {
            SimpleRNG.SetSeedFromSystemTime();
            Restart();
        }
        
        public void OnTime()
        {
            ModelTime += TIME_PER_STEP;
            foreach (Person p in Patients)
            {
                p.onTime(TIME_PER_STEP.TotalSeconds, ModelTime);
            }
        }

        public void CreatePatients()
        {
            Patients = new List<Person>();

            Person vasya = PersonFactory.createVasya(Building.Instance.Floor.getRoom("1"), Building.Instance.Floor.GetFloorKnowledge());
            Patients.Add(vasya);
            

            Patients.ForEach(p => { p.ActionChanged += patientHandler; });
        }

        private void patientHandler(Person person, PersonAction action)
        {
            if (ModelEventOccurred != null)
                ModelEventOccurred(ModelTime.ToString(@"hh\:mm") + "\t" + person.Name + " get state '" + action + "'");
        }

        public IEnumerable<Person> getPatients()
        {
            return Patients;
        }

        /// <summary>
        /// Skips some time
        /// </summary>
        /// <param name="hours">time in hours</param>
        public void skip(double hours)
        {
            Logger.Log("Skip started (" + hours + ")");
            DateTime startTime = DateTime.Now;
            int seconds = (int)(hours * 3600);
            for (int i = 0; i < seconds; i++)
                OnTime();
            Logger.Log("Skip finished (time=" + (DateTime.Now - startTime).TotalSeconds + ")");
        }

        public void Restart()
        {
            ModelTime = new TimeSpan(12, 0, 0);
        }
    }
}
