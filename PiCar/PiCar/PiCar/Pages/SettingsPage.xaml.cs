using System;
using System.Collections.Generic;
using System.Linq;
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
            CloseThis();
        }

	    private void DeleteClick(object sender, EventArgs e)
	    {
            settings.DeleteServer();
            CloseThis();
        }

	    private void AddClick(object sender, EventArgs e)
	    {
	        settings.AddServer();
	    }

	    private void ServersChanged(object sender, EventArgs e)
	    {
	        settings.LoadServer(Servers.Items[Servers.SelectedIndex]);
            AppPage.SelectedServer = Servers.Items[Servers.SelectedIndex];
        }

        protected override bool OnBackButtonPressed()
        {
            CloseThis();
            return true;
        }

	    private void CloseThis()
	    {
            Settings.IsOpen = false;
            Navigation.PopAsync(true);
        }
	}
}
