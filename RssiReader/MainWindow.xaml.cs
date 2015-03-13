using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LocalizationCore;
using LocalizationCore.BuildingModel;
using LocalizationCore.Localization.Locations;
using LocalizationCore.Localization;
using NativeWifi;
using LocalizationCore.SensorModel.Devices;
using LocalizationCore.SensorModel;
using RssiReader.Wifi;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RssiReader.ViewModel;
using LogProvider;
using System.Timers;
using LocalizationCore.Localization.Map;

namespace RssiReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string SAVED_FILE = "locations.dat";
        const bool FROM_FILE = false;
        const int COUNT_THRESHOLD = 100;

        ModelPainter painter;
        LocationInfoVM LocationInfoVM { get; set; }
        Floor Floor { get; set; }

        bool touch = false;
        bool mouse = true;

        Timer Timer = new Timer();

        public MainWindow()
        {            
            Logger.Log("RssiReader Launched!");
            InitializeComponent();
            Building.CreateBuilding(FloorInstance.Office);
            Floor = Building.Instance.Floor;
            painter = new ModelPainter(Model.Instance, Floor, TheCanvas);

            LocationInfoVM = new LocationInfoVM() { Map = new RadioMap(), 
                                                    SelectedLocation = null,
                                                    Devices = new System.Collections.ObjectModel.ObservableCollection<DeviceVM>() };
            
            if (FROM_FILE)
                TryLoad();

            LocationInfoVM.NeedRedrawMap += LocationInfoVM_NeedRedrawMap;

            LocationView.DataContext = LocationInfoVM;

            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        Fingerprint lastFingerprint = null;
        int sameCounter = 0;
        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var newFingerprint = Measurer.Instance.GetFingerprint();
            if (lastFingerprint != null && !newFingerprint.Same(lastFingerprint))
            {
                sameCounter = 0;
                LocationInfoVM.OnlineMode = true;
            }
            else
            {
                sameCounter++;
                if (sameCounter > COUNT_THRESHOLD)
                    LocationInfoVM.OnlineMode = false;
            }

            lastFingerprint = newFingerprint;
        }

        private void LocationInfoVM_NeedRedrawMap()
        {
            Redraw();
        }

        private void TryLoad()
        {
            if (File.Exists(SAVED_FILE))
            {
                FileStream stream = null;
                try
                {
                    stream = new FileStream(SAVED_FILE, FileMode.Open);
                    var formatter = new BinaryFormatter();
                    LocationInfoVM = (LocationInfoVM)formatter.Deserialize(stream);
                    stream.Close();
                }
                catch
                {
                    if (stream != null)
                    {
                        stream.Close();
                        File.Delete(SAVED_FILE);
                        MessageBox.Show("Cannot read saved file", "Read error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                        throw;
                }
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            Redraw();
        }

        public void Redraw(bool repaint = true)
        {
            painter.DrawObservations(LocationInfoVM.Map, LocationInfoVM.SelectedDevice, 
                        LocationInfoVM.SelectedLocation == null ? null : LocationInfoVM.SelectedLocation.OriginalLocation, repaint);
            painter.DrawFloor();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!touch)
            {
                Logger.Log("MouseDown", LoggingLevel.Trace);
                OnPoint(e.GetPosition(TheCanvas));
                mouse = true;
            }            
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            if (!mouse)
            {
                Logger.Log("TouchDown", LoggingLevel.Trace);
                OnPoint(e.GetTouchPoint(TheCanvas).Position);
                touch = true;
            }
        }

        private void OnPoint(Point p)
        {
            Timing timing = Timing.Start("OnPoint");
            var selected = GetLocation(p);

            if (selected != null)
            {
                LocationInfoVM.SelectedLocation = new LocationVM() { OriginalLocation = selected, DevicesRef = LocationInfoVM.Devices };
                Redraw(false);
                LocationView.DataContext = LocationInfoVM;
            }
            timing.Finish(LoggingLevel.Debug);
        }

        private Observations GetLocation(Point point)
        {
            throw new NotImplementedException();
            //Timing timing = Timing.Start("GetLocation");
            var res = LocationInfoVM.Map[LocationMap.Instance.FindLocation(painter.ToCoord(point))];
            //timing.Finish(LoggingLevel.Debug);
            return res;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveMap();
        }

        private void SaveMap()
        {
            try
            {
                if (File.Exists(SAVED_FILE))
                    File.Delete(SAVED_FILE);

                var stream = new FileStream(SAVED_FILE, FileMode.CreateNew);
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, LocationInfoVM);
                stream.Close();
            }
            catch
            {
                MessageBox.Show("Cannot save data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}
