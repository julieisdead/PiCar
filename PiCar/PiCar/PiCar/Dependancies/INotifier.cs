using Android.Widget;

namespace PiCar
{
    public enum ToastPriority
    {
        Low,
        High,
        Critical
    }

    internal interface INotifier
    {
        void MakeToast(string message, ToastPriority priority, ToastLength Length);
    }
}
