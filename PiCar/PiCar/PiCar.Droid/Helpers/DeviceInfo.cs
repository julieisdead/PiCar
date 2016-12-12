using PiCar.Droid;
using Xamarin.Forms;
using XLabs.Platform.Device;

[assembly: Dependency(typeof(DeviceInfo))]
namespace PiCar.Droid
{
    internal class DeviceInfo : IDeviceInfo
    {
        public DeviceInfo() { }

        public string GetName() => AndroidDevice.CurrentDevice.Name;
    }
}