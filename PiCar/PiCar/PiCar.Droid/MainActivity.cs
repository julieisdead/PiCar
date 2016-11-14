using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Xamarin.Forms;
using XLabs.Forms;

namespace PiCar.Droid
{
	[Activity (MainLauncher = true, NoHistory = true, Theme = "@style/Theme.Splash", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : XFormsApplicationDroid
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            Forms.Init(this, bundle);
            LoadApplication(new App());
		}
	}
}


