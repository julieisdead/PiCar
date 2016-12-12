using System;
using System.Collections.Generic;
using Android.Content.PM;
using Xamarin.Forms;

namespace PiCar
{
	public partial class SettingsPage : ContentPage
	{
	    private readonly Settings settings;
	    private double ControlsHeight;

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

	    protected override void OnAppearing()
	    {
	        base.OnAppearing();
            DependencyService.Get<IOrientate>().SetOrientation(ScreenOrientation.Portrait);
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

	    private void AddClick(object sender, EventArgs e)
        {
            if (EnableControlsCheckBox.Checked)
                AnimateControls(false);
            settings.AddServer();
	    }

	    private void ServersChanged(object sender, EventArgs e)
	    {
	        AppPage.SelectedServer = Servers.Items[Servers.SelectedIndex];
	        settings.LoadServer(Servers.Items[Servers.SelectedIndex]);
	    }

	    private void EnableControlsCheckBoxCheckChanged(object sender, EventArgs e)
	        => AnimateControls(EnableControlsCheckBox.Checked);


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

	    private void ControlsLayoutLayoutChanged(object sender, EventArgs e)
        {
            if (Math.Abs(ControlsHeight) < 0.01f)
                ControlsHeight = LayoutUserName.Height;

            if (EnableControlsCheckBox.Checked)
            {
                LayoutUserName.HeightRequest = ControlsHeight;
                LayoutMqttPort.HeightRequest = ControlsHeight;
                LayoutPassword.HeightRequest = ControlsHeight;
            }
            else
            {
                LayoutUserName.HeightRequest = -10;
                LayoutMqttPort.HeightRequest = -10;
                LayoutPassword.HeightRequest = -10;
            }
        }

	    private void AnimateControls(bool show)
        {
            Action<double> callBack = (input) =>
            {
                LayoutUserName.HeightRequest = input;
                LayoutPassword.HeightRequest = input;
                LayoutMqttPort.HeightRequest = input;
            };
            double startHeight;
            double endingHeight;

            if (show)
            {
                startHeight = -10;
                endingHeight = ControlsHeight;
            }
            else
            {
                startHeight = LayoutUserName.Height;
                ControlsHeight = LayoutUserName.Height;
                endingHeight = -10;
            }
            this.Animate("toggle", callBack, startHeight, endingHeight, 8, 1000, Easing.CubicInOut);
        }
	}
}
