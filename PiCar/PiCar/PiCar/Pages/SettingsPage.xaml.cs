using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

	    private async void DeleteClick(object sender, EventArgs e)
	    {
	        bool removeYes = await OnAlertYesNoClicked();
	        if (removeYes)
	        {
	            settings.DeleteServer();
	            CloseThis();
	        }
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

        private async Task<bool> OnAlertYesNoClicked()
        {
            bool answer = await DisplayAlert("Remove Server?", "Do you want to permanently remove this server?", "Yes", "No");
            return answer;
        }
    }
}
