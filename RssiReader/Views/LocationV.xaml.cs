using LocalizationCore.Localization;
using LocalizationCore.SensorModel;
using RssiReader.ViewModel;
using RssiReader.Wifi;
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

namespace RssiReader.Views
{
    /// <summary>
    /// Interaction logic for LocationV.xaml
    /// </summary>
    public partial class LocationV : UserControl
    {
        public LocationV()
        {
            InitializeComponent();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            var locationInfo = (LocationInfoVM)DataContext;
            var selected = lbxFingerprints.SelectedItem;
            if (selected != null)
            {
                locationInfo.SelectedLocation.RemoveFingerprint((FingerprintVM)selected);
                if (locationInfo.SelectedLocation.Fingerprints.Count() != 0)
                    lbxFingerprints.SelectedIndex = 0;
            }
            else
                MessageBox.Show("Choose fingerprint to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            var locationInfo = (LocationInfoVM)DataContext;
            if (locationInfo.SelectedLocation != null)
            {
                var fingerprint = Measurer.Instance.GetFingerprint();
                UpdateDeviceList(fingerprint, locationInfo);
                locationInfo.SelectedLocation.AddFingerprint(fingerprint);

                ScrollDown();
            }
            else
                MessageBox.Show("Choose location", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void ScrollDown()
        {
            lbxFingerprints.SelectedIndex = lbxFingerprints.Items.Count - 1;
            lbxFingerprints.ScrollIntoView(lbxFingerprints.Items[lbxFingerprints.Items.Count - 1]);
        }

        private void UpdateDeviceList(Fingerprint fingerprint, LocationInfoVM locationInfo)
        {
            foreach (var rss in fingerprint.Rss)
                if (!locationInfo.Devices.Any(d => d.Device.Name == rss.Key.Name))
                    locationInfo.Devices.Add(new DeviceVM() { Device = rss.Key, Number = locationInfo.Devices.Count() + 1 });
        }

        private void lbxAccessPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxAccessPoints.SelectedValue != null)
            {
                var context = (LocationInfoVM)DataContext;
                context.SelectedDevice = ((DeviceVM)lbxAccessPoints.SelectedValue).Device;
                context.RedrawMap();
            }
        }

        private void EstimateLocationClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
