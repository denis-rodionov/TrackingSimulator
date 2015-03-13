using LocalizationCore.Localization;
using LocalizationCore.Localization.Locations;
using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RssiReader.ViewModel
{
    [Serializable]
    class LocationInfoVM : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public Device SelectedDevice { get; set; }

        //public <Location> SelectedLocation { get; set; }
        public RadioMap Map { get; set; }

        [NonSerialized]
        LocationVM location;
        public LocationVM SelectedLocation {
            get { return location; }
            set
            {
                location = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedLocation"));
            }
        }
        
        public ObservableCollection<DeviceVM> Devices { get; set; }

        [field: NonSerializedAttribute()] 
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerializedAttribute()] 
        public event Action NeedRedrawMap;

        public void RedrawMap()
        {
            if (NeedRedrawMap != null)
                NeedRedrawMap();
        }

        [NonSerialized]
        bool onlineMode;
        public bool OnlineMode
        {
            get { return onlineMode; }
            set
            {
                onlineMode = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OnlineMode"));
            }
        }
    }
}
