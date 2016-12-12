using Android.Content.PM;
using PiCar.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Orientate))]
namespace PiCar.Droid
{
    internal class Orientate : IOrientate
    {
        public void SetOrientation(ScreenOrientation orientation)
        {
            MainActivity activity = Forms.Context as MainActivity;
            if(activity != null)
                activity.RequestedOrientation = orientation;
        }
    }
}