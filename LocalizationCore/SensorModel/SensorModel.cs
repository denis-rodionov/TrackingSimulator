using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalizationCore.SensorModel.Devices;

namespace LocalizationCore.SensorModel
{
    class SensorModel
    {
        #region Data 
        /// <summary>
        /// Singletone instance
        /// </summary>
        static SensorModel instance;

        /// <summary>
        /// List of Access points, RFID readers, BSs, etc.
        /// </summary>
        //public List<Device> deviceList = new List<Device>();

        /// <summary>
        /// Sensors attached to people
        /// </summary>
        //public List<RssSensor> sensors = new List<RssSensor>();

        #endregion

        /// <summary>
        /// Singletone accessor
        /// </summary>
        public static SensorModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new SensorModel();
                return instance;
            }
        }

        /// <summary>
        /// Private contrructor
        /// </summary>
        private SensorModel()
        {
        }

        /// <summary>
        /// Add device to the model
        /// </summary>
        /// <param name="device"></param>
        //public void AddDevice(Device device)
        //{
        //    deviceList.Add(device);
        //}
    }
}
