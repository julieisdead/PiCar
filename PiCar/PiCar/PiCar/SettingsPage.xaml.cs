using System;
using Xamarin.Forms;

namespace PiCar
{
	public partial class SettingsPage : ContentPage
	{
	    private Settings settings;

        public SettingsPage(string name)
		{
		    InitializeComponent();
            settings = Settings.LoadSettings(name);
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = settings;
		}

        private void SaveClick(object sender, EventArgs e)
        {
            settings.SaveServer();
            Settings.IsOpen = false;
            Navigation.PopAsync(true);
        }

	    private void DeleteClick(object sender, EventArgs e)
	    {
            settings.DeleteServer();
            Settings.IsOpen = false;
            Navigation.PopAsync(true);
        }

	    private void AddClick(object sender, EventArgs e)
	    {
	        settings.AddServer();
	    }
	}
}
