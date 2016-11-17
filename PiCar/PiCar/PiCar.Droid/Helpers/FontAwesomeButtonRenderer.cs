using Android.Graphics;
using PiCar.Droid;
using PiCar;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(FontAwesomeButton), typeof(FontAwesomeButtonRenderer))]

namespace PiCar.Droid
{
    /// <summary>
    /// Add the FontAwesome.ttf to the Assets folder and mark as "Android Asset"
    /// </summary>
    public class FontAwesomeButtonRenderer : ButtonRenderer
    {
        public FontAwesomeButtonRenderer() {}

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (changed)
            {
                Control.Typeface = Typeface.CreateFromAsset(Forms.Context.Assets, FontAwesomeButton.Typeface + ".ttf");
            }
        }
    }
}