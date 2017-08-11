using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Serialization;

namespace PiCar
{
	internal class App : Application
	{
        private AppPage appPage;

		internal App()
        {
            SetIoc();
            appPage = new AppPage();
            MainPage = new NavigationPage(appPage);
		}

        protected override void OnSleep()
        {
            base.OnSleep();
            appPage.Disconnect();
        }

        protected override void OnResume()
        {
            base.OnResume();
            appPage.Connect();
        }

        internal void SetIoc()
        {
            if (Resolver.IsSet) return;
            SimpleContainer resolverContainer = new SimpleContainer();
            resolverContainer.Register<IJsonSerializer, XLabs.Serialization.JsonNET.JsonSerializer>();
            Resolver.SetResolver(resolverContainer.GetResolver());
        }
    }
}

