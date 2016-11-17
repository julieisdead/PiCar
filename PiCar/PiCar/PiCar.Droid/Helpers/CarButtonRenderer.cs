using Android.Views;
using PiCar;
using PiCar.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CarButton), typeof(CarButtonRenderer))]
namespace PiCar.Droid
{
    public class CarButtonRenderer : ButtonRenderer
    {
        public override bool DispatchTouchEvent(MotionEvent e)
        {
            CarButton target = (CarButton) Element;
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    target.RaiseTouched(true);
                    break;
                case MotionEventActions.Up:
                    target.RaiseTouched(false);
                    break;
            }
            return true;
        }
    }
}