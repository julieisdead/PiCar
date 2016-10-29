using System;
using Xamarin.Forms;

namespace PiCar
{
	public partial class SettingsPage : ContentPage
	{
	    public static bool IsOpen;

		public SettingsPage ()
		{
			InitializeComponent ();
		    Settings settings = Settings.LoadSettings();
            NavigationPage.SetHasBackButton(this, false);
            BindingContext = settings;
		}

        private void BackClick(object sender, EventArgs e)
        {
            if(!IsOpen) return;
            IsOpen = false;
            Navigation.PopAsync(true);
        }

	    protected override void OnAppearing()
        {
            IsOpen = true;
            base.OnAppearing();
	    }
	}
}
