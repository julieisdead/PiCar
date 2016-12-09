using Android.Content.PM;
using PiCar;
using PiCar.Droid;
using PiCar.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(Orientate))]
namespace PiCar.Helpers
{
    class Orientate : IOrientate
    {
        public void SetOrientation(ScreenOrientation orientation)
        {
            MainActivity activity = Forms.Context as MainActivity;
            if(activity != null)
                activity.RequestedOrientation = orientation;
        }
    }
}