using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PiCar
{
	public partial class SettingsPage : ContentPage
	{
	    private readonly Settings settings;

        public SettingsPage(string name)
		{
		    InitializeComponent();
            BackgroundColor = Color.FromHex("#CFD8DC");
            settings = Settings.LoadSettings(name);
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            List<string> servers = Settings.GetServers();
            Servers.Items.Clear();
            foreach (string item in servers)
                Servers.Items.Add(item);
            if(Servers.Items.Contains(name))
                Servers.SelectedIndex = Servers.Items.IndexOf(name);
            BindingContext = settings;
		}

        private void SaveClick(object sender, EventArgs e)
        {
            settings.SaveServer();
            AppPage.SelectedServer = settings.Name;
            CloseThis();
        }

	    private async void DeleteClick(object sender, EventArgs e)
	    {
	        bool removeYes =
	            await DisplayAlert("Remove Server?", "Do you want to permanently remove this server?", "Yes", "No");
            if (removeYes)
	        {
	            settings.DeleteServer();
	            AppPage.SelectedServer = string.Empty;
                CloseThis();
	        }
	    }

	    private void AddClick(object sender, EventArgs e) => settings.AddServer();

	    private void ServersChanged(object sender, EventArgs e)
	    {
	        AppPage.SelectedServer = Servers.Items[Servers.SelectedIndex];
	        settings.LoadServer(Servers.Items[Servers.SelectedIndex]);
	    }

	    protected override bool OnBackButtonPressed()
        {
            CloseThis();
            return true;
        }

	    private void CloseThis()
	    {
            if(Settings.IsOpen == false) return;
            Settings.IsOpen = false;
            Navigation.PopAsync(true);
        }
    }
}
