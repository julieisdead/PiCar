using Android.App;
using Android.OS;
using Refractored.XamForms.PullToRefresh.Droid;
using Xamarin.Forms;
using XLabs.Forms;

namespace PiCar.Droid
{
	[Activity (Label = "PiCar", MainLauncher = true, Icon = "@drawable/Icon", Theme = "@style/Theme.Car")]
	public class MainActivity : XFormsApplicationDroid
    {
        protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate (bundle);
            Forms.Init(this, bundle);
            PullToRefreshLayoutRenderer.Init();
            LoadApplication(new App());
		}
	}
}


