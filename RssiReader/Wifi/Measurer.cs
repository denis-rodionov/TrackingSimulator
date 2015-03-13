using LocalizationCore.SensorModel;
using LocalizationCore.SensorModel.Devices;
using LogProvider;
using NativeWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssiReader.Wifi
{
    class Measurer
    {
        WlanClient WifiClient { get; set; }

        static Measurer _instance;

        private Measurer()
        {
            WifiClient = new WlanClient();
        }

        public static Measurer Instance { 
            get {
                if (_instance == null)
                    _instance = new Measurer();
                return _instance;
            }
        }

        public Fingerprint GetFingerprint()
        {
            try
            {
                //Wlan.WlanBssEntry
                var bss = WifiClient.Interfaces.First().GetNetworkBssList();
                
                //var list = WifiClient.Interfaces.First().GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles);     //.ToList();
                var aps = bss.Select(ap => new Tuple<string, double>(BytesToString(ap.dot11Ssid.SSID) + " - " + BssidToString(ap.dot11Bssid), (double)ap.rssi)).ToList();
                //DeleteDuplicates(aps);
                //DeleteHidden(aps);

                var fingerprint = new Fingerprint(aps.ToDictionary(ap => (Device)new WifiAccessPoint(ap.Item1, null), ap => ap.Item2));
                return fingerprint;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw;
            }
        }

        private void DeleteHidden(List<Tuple<string, double>> list)
        {
            for (int i = 0; i < list.Count(); i++)
                if (list[i].Item1 == string.Empty)
                    list.Remove(list[i]);
        }

        public static string BytesToString(byte[] ssid)
        {
            string res = System.Text.Encoding.Default.GetString(ssid);
            if (res.IndexOf('\0') != -1)
                res = res.Substring(0, res.IndexOf('\0'));
            return res;
        }

        public static string BssidToString(byte[] bssid)
        {
            var macAddrLen = (uint)bssid.Length;
            var str = new string[(int)macAddrLen];
            for (int i = 0; i < macAddrLen; i++)
            {
                str[i] = bssid[i].ToString("x2");
            }
            string mac = string.Join("", str);
            return mac;
        }

        private void DeleteDuplicates(List<Tuple<string, double>> list)
        {
            bool duplicetesExists;
            while (true)
            {
                duplicetesExists = false;
                var items = new List<string>();
                for (int i = 0; i < list.Count(); i++)
                {
                    var current = list.ElementAt(i).Item1;
                    if (items.Exists(s => s == current))
                    {
                        list.Remove(list.ElementAt(i));
                        duplicetesExists = true;
                        break;
                    }
                    else
                        items.Add(current);
                }
                if (!duplicetesExists)
                    break;
            }
        }
    }
}
