using Android.Widget;
using PiCar.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Notifier))]
namespace PiCar.Droid
{
    public class Notifier : INotifier
    {
        public Notifier() { }

        public void MakeToast(string message)
            => Device.BeginInvokeOnMainThread(()
                => Toast.MakeText(Forms.Context, message, ToastLength.Short).Show());
    }
}