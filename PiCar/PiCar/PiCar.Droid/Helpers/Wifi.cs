using Android.Content;
using Android.Net.Wifi;
using PiCar.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Wifi))]
namespace PiCar.Droid
{

    internal class Wifi : IWifi
    {
        public Wifi() { }

        public string GetSSID()
        {
            WifiManager wifiManager =
                (WifiManager) Android.App.Application.Context.GetSystemService(Context.WifiService);
            WifiInfo wifiInfo = wifiManager.ConnectionInfo;
            return wifiInfo.SSID;
        }
    }
}