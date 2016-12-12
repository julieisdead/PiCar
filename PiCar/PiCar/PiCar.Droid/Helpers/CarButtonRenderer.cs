using Android.Graphics;
using Android.Views;
using PiCar;
using PiCar.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CarButton), typeof(CarButtonRenderer))]
namespace PiCar.Droid
{
    internal class CarButtonRenderer : ButtonRenderer
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
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (changed)
            {
                Control.Typeface = Typeface.CreateFromAsset(Forms.Context.Assets, CarButton.Typeface + ".ttf");
            }
        }
    }
}