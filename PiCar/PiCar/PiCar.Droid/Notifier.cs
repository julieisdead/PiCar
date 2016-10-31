using Android.Widget;
using PiCar.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Notifier))]
namespace PiCar.Droid
{
    public class Notifier : INotifier
    {
        public static Toast toast;

        public Notifier() { }

        public void MakeToast(string message) => Device.BeginInvokeOnMainThread(() =>
        {
            toast?.Cancel();
            toast = Toast.MakeText(Forms.Context, message, ToastLength.Short);
            toast.Show();
        });
    }
}