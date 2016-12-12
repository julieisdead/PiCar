using PiCar.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl_Android))]
namespace PiCar.Droid
{
    internal class BaseUrl_Android : IBaseUrl
    {
        public string Get() => "file:///android_asset/";
    }
}