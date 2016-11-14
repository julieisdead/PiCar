using System;
using Xamarin.Forms;

namespace PiCar
{
	public partial class ServerPage : ContentPage
	{
		public ServerPage()
		{
		    InitializeComponent ();
            Server settings = Server.LoadSettings();
            NavigationPage.SetHasBackButton(this, false);
            BindingContext = settings;
		}

        private void BackClick(object sender, EventArgs e)
        {
            Server.IsOpen = false;
            Navigation.PopAsync(true);
        }

	    private void Restart_OnClicked(object sender, EventArgs e) => AppPage.SendRestartCommand();
	}
}
