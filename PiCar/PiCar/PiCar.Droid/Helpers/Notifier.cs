using Android.Widget;
using PiCar.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Notifier))]
namespace PiCar.Droid
{
    public class Notifier : INotifier
    {
        public static Toast toast;
        public static ToastPriority Priority;
        public Notifier() { }

        public void MakeToast(string message, ToastPriority priority, ToastLength length) => Device.BeginInvokeOnMainThread(() =>
        {
            if (Priority == ToastPriority.Low)
                toast?.Cancel();

            if(priority == ToastPriority.High && priority == ToastPriority.High)
                toast?.Cancel();

            if(toast != null && !toast.View.IsShown)
                Priority = priority;

            toast = Toast.MakeText(Forms.Context, message, length);
            toast.Show();
        });
    }
}