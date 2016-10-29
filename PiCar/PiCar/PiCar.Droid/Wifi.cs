using Android.Content;
using Android.Net.Wifi;
using PiCar.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Wifi))]
namespace PiCar.Droid
{

    public class Wifi : IWifi
    {
        public Wifi() { }

        public string GetSSID()
        {
            var wifiManager = (WifiManager)(Android.App.Application.Context.GetSystemService(Context.WifiService));
            var wifiInfo = wifiManager.ConnectionInfo;
            return wifiInfo.SSID;
        }
    }
}