using System;
using Xamarin.Forms;

namespace PiCar
{
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
		    InitializeComponent ();
		    Settings settings = Settings.LoadSettings();
            NavigationPage.SetHasBackButton(this, false);
            BindingContext = settings;
		}

        private void BackClick(object sender, EventArgs e)
        {
            Settings.IsOpen = false;
            Navigation.PopAsync(true);
        }
    }
}
