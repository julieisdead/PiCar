using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Serialization;

namespace PiCar
{
	public class App : Application
	{
		public App()
        {
            SetIoc();
            MainPage = new NavigationPage(new AppPage());
		}

        public void SetIoc()
        {
            if (Resolver.IsSet) return;
            SimpleContainer resolverContainer = new SimpleContainer();
            resolverContainer.Register<IJsonSerializer, XLabs.Serialization.JsonNET.JsonSerializer>();
            Resolver.SetResolver(resolverContainer.GetResolver());
        }
    }
}

