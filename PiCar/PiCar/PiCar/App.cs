using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Serialization;

namespace PiCar
{
	internal class App : Application
	{
		internal App()
        {
            SetIoc();
            MainPage = new NavigationPage(new AppPage());
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

