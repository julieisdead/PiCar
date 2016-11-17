using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MqttLib;
using PiCar.Droid;
using Xamarin.Forms;

namespace PiCar
{
    public partial class AppPage : ContentPage
    {
        private Movement movement;
        private static IMqtt client;

        public AppPage()
        {
            InitializeComponent();
            Title = "PiCar";

            movement = new Movement();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            List<string> servers = Settings.GetServers();
            Servers.Items.Clear();
            foreach (string item in servers)
                Servers.Items.Add(item);
            if (Servers.Items.Count > 0)
                Servers.SelectedIndex = 0;
            Connect();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            movement = new Movement();
            SendToMosquitto(movement.ToString());
        }

        private void EditClicked(object sender, EventArgs e) => ShowSettingsPage();

        private void RefreshClicked(object sender, EventArgs e) => Connect();

        private void ServersChanged(object sender, EventArgs e)
        {
            if(Servers.SelectedIndex >= 0)
                Connect();
        }

        private void OnForwardTouched(object sender, EventArgs e)
        {
            if (movement.Reverse) return;
            movement.Forward = ((CarButton) sender).State == CarButtonState.Down;
            MoveCar();
        }

        private void OnReverseTouched(object sender, EventArgs e)
        {
            if (movement.Forward) return;
            movement.Reverse = ((CarButton) sender).State == CarButtonState.Down;
            MoveCar();
        }

        private void OnLeftTouched(object sender, EventArgs e)
        {
            if (movement.Right) return;
            movement.Left = ((CarButton) sender).State == CarButtonState.Down;
            MoveCar();
        }

        private void OnRightTouched(object sender, EventArgs e)
        {
            if (movement.Left) return;
            movement.Right = ((CarButton) sender).State == CarButtonState.Down;
            MoveCar();
        }

        private double pageWidth;
        private double pageHeight;

        protected override void OnSizeAllocated(double newWidth, double newHeight)
        {
            base.OnSizeAllocated(newWidth, newHeight);
            if (Math.Abs(newWidth - pageWidth) > 0 || Math.Abs(newHeight - pageHeight) > 0)
            {
                pageWidth = newWidth;
                pageHeight = newHeight;
                if (newWidth > newHeight)
                {
                    NavigationPage.SetHasNavigationBar(this, false);
                    MainLayout.Children.Clear();

                    //Car Cam View
                    MainLayout.Children.Add(CamWebView,
                        Constraint.RelativeToParent((parent) => parent.X + parent.Width*0.15),
                        Constraint.RelativeToParent((parent) => parent.Y),
                        Constraint.RelativeToParent((parent) => parent.Width*0.7),
                        Constraint.RelativeToParent((parent) => parent.Height));

                    // StatusText
                    MainLayout.Children.Add(StatusText,
                        Constraint.RelativeToParent((parent) => parent.X + parent.Width*0.15 + 20),
                        Constraint.RelativeToParent((parent) => parent.Height - StatusText.Height - 5),
                        Constraint.RelativeToParent((parent) => parent.Width*0.7),
                        Constraint.Constant(StatusText.Height));

                    MainLayout.Children.Add(ForwardButton,
                        Constraint.RelativeToParent((parent) => parent.X + 20),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.2 + 20),
                        Constraint.Constant(75),
                        Constraint.Constant(75));

                    MainLayout.Children.Add(ReverseButton,
                        Constraint.RelativeToParent((parent) => parent.X + 60),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.2 + 115),
                        Constraint.Constant(75),
                        Constraint.Constant(75));

                    MainLayout.Children.Add(RightButton,
                        Constraint.RelativeToParent((parent) => parent.Width - 95),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.2 + 20),
                        Constraint.Constant(75),
                        Constraint.Constant(75));

                    MainLayout.Children.Add(LeftButton,
                        Constraint.RelativeToParent((parent) => parent.Width - 135),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.2 + 115),
                        Constraint.Constant(75),
                        Constraint.Constant(75));
                }
                else
                {
                    NavigationPage.SetHasNavigationBar(this, false);
                    MainLayout.Children.Clear();

                    MainLayout.Children.Add(FakeToolbar,
                        Constraint.Constant(0),
                        Constraint.Constant(0),
                        Constraint.RelativeToParent((parent) => parent.Width),
                        Constraint.Constant(50));

                    FakeToolbar.Children.Clear();
                    FakeToolbar.Children.Add(Servers,
                        Constraint.RelativeToParent((parent) => parent.X + 10),
                        Constraint.RelativeToParent((parent) => parent.Height * 0.5 - Servers.Height * 0.5),
                        Constraint.RelativeToParent((parent) => parent.Width * 0.5));

                    FakeToolbar.Children.Add(EditButton, 
                        Constraint.RelativeToParent((parent) => parent.Width - 60),
                        Constraint.RelativeToParent((parent) => parent.Height * 0.5 - EditButton.Height * 0.5));

                    FakeToolbar.Children.Add(RefreshButton,
                        Constraint.RelativeToParent((parent) => parent.Width - 105),
                        Constraint.RelativeToParent((parent)=> parent.Height * 0.5 - RefreshButton.Height * 0.5));

                    // Car Cam View
                    MainLayout.Children.Add(CamWebView,
                        Constraint.RelativeToParent((parent) => parent.X),
                        Constraint.RelativeToParent((parent) => parent.Y + 50),
                        Constraint.RelativeToParent((parent) => parent.Width),
                        Constraint.RelativeToParent((parent) => parent.Height*0.5));

                    // StatusText
                    MainLayout.Children.Add(StatusText,
                        Constraint.RelativeToParent((parent) => parent.X + 20),
                        Constraint.RelativeToParent((parent) => parent.Height*0.5 - StatusText.Height*1.5 + 50),
                        Constraint.RelativeToParent((parent) => parent.Width),
                        Constraint.Constant(StatusText.Height));

                    MainLayout.Children.Add(ForwardButton,
                        Constraint.RelativeToParent((parent) => parent.X + 20),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.5 + 60),
                        Constraint.Constant(75),
                        Constraint.Constant(75));

                    MainLayout.Children.Add(ReverseButton,
                        Constraint.RelativeToParent((parent) => parent.X + 60),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.5 + 155),
                        Constraint.Constant(75),
                        Constraint.Constant(75));

                    MainLayout.Children.Add(RightButton,
                        Constraint.RelativeToParent((parent) => parent.Width - 95),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.5 + 60),
                        Constraint.Constant(75),
                        Constraint.Constant(75));

                    MainLayout.Children.Add(LeftButton,
                        Constraint.RelativeToParent((parent) => parent.Width - 135),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.5 + 155),
                        Constraint.Constant(75),
                        Constraint.Constant(75));
                }
                ConnectToCam();
            }
        }

        private void Connect()
        {
            ConnectToBroker();
            ConnectToCam();
        }

        private void ConnectToCam()
        {
            if (Servers.SelectedIndex < 0)
            {
                ShowSettingsPage();
                return;
            }

            Settings settings = Settings.LoadSettings(Servers.Items[Servers.SelectedIndex]);
            IWifi wifi = new Wifi();

            string server = wifi.GetSSID() == $"\"{settings.LocalSSID}\""
                ? settings.LocalServerName
                : settings.RemoteServerName;

            if (string.IsNullOrWhiteSpace(server))
            {
                ShowSettingsPage();
                return;
            }

            string html = "<html><head><style>" +
                          $"body{{Width:{CamWebView.Width - 16}px;Height:{CamWebView.Height - 16}px;}}" +
                          $".loader{{left:{CamWebView.Width/2 - 8};margin:{CamWebView.Height/2 - 46}px auto;position:fixed;}}" +
                          $"img{{width:{CamWebView.Width - 16};height:{CamWebView.Height - 16};position:fixed;}}" +
                          "</style><link rel=\"stylesheet\" type=\"text/css\" href=\"car-cam.css\" /></head><body>" +
                          "<div class=\"loader\">Loading...</div>" +
                          $"<img class=\"camview\" src=\"http://{server}:{settings.CameraPort}/test.mjpg\" onerror=\"this.src = '';\" />" +
                          "</body></html>";

            CamWebView.LoadContent(html, DependencyService.Get<IBaseUrl>().Get());
        }

        private void ConnectToBroker()
        {
            if (Servers.SelectedIndex < 0)
            {
                ShowSettingsPage();
                return;
            }
            Settings settings = Settings.LoadSettings(Servers.Items[Servers.SelectedIndex]);
            IWifi wifi = new Wifi();

            string server = wifi.GetSSID() == $"\"{settings.LocalSSID}\""
                ? settings.LocalServerName
                : settings.RemoteServerName;

                     if (string.IsNullOrWhiteSpace(server))
                     {
                         ShowSettingsPage();
                         return;
                     }
            string connectionString = $"tcp://{server}:{settings.MqttPort}";
            try
            {
                if (string.IsNullOrEmpty(settings.Username))
                {
                    client = MqttClientFactory.CreateClient(connectionString,
                        Guid.NewGuid().ToString());
                }
                else
                {
                    client = MqttClientFactory.CreateClient(connectionString,
                        Guid.NewGuid().ToString(), settings.Username, settings.Password);
                }
                client.Connected += client_Connected;
                client.ConnectionLost += client_ConnectionLost;
                client.PublishArrived += client_PublishArrived;
                Task.Run(
                    () =>
                        client.Connect("car/DISCONNECT", QoS.AtLeastOnce,
                            XLabs.Platform.Device.AndroidDevice.CurrentDevice.Name, false, true));
            }
            catch
            {
                //
            }
        }

        private static void SendToMosquitto(string value)
        {
            try
            {
                MqttPayload payload = new MqttPayload(value);
                client.Publish("car/REQUEST", payload, QoS.AtLeastOnce, false);
            }
            catch
            {
                //
            }
        }

        public static void SendRestartCommand()
        {
            try
            {
                MqttPayload paylod = new MqttPayload("1");
                client.Publish("cam/RESTART", paylod, QoS.AtLeastOnce, false);
            }
            catch
            {
                //
            }
        }

        private void client_Connected(object sender, EventArgs e) => Device.BeginInvokeOnMainThread(() =>
        {
            RegisterOurSubscriptions();

            movement = new Movement();
            SendToMosquitto(movement.ToString());
            client.Publish("car/CONNECT", XLabs.Platform.Device.AndroidDevice.CurrentDevice.Name, QoS.BestEfforts, false);
        });

        private static void client_ConnectionLost(object sender, EventArgs e) => Toaster("Client disconnected");

        private static void RegisterOurSubscriptions()
        {
            try
            {
                client.Subscribe("car/RESPOND", QoS.BestEfforts);
                client.Subscribe("car/RECONCAM", QoS.BestEfforts);
                client.Subscribe("car/STATE", QoS.BestEfforts);
            }
            catch
            {
                //
            }
        }

        private bool client_PublishArrived(object sender, PublishArrivedArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                string response = e.Payload.ToString();
                switch (e.Topic)
                {
                    case "car/RECONCAM":
                        ConnectToCam();
                        return;
                    case "car/STATE":
                        StatusText.Text = response;
                        return;
                    case "car/RESPOND":
                        Toaster(response);
                        break;
                }
            });
            return true;
        }

        private void ShowSettingsPage()
        {
            if (Settings.IsOpen) return;
            Settings.IsOpen = true;
            string server = string.Empty;
            if(Servers.SelectedIndex >= 0)
                if (Servers.Items.Count > 0)
                    server = Servers.Items[Servers.SelectedIndex];
            Navigation.PushAsync(new SettingsPage(server), true);
        }

        private void MoveCar() => SendToMosquitto(movement.ToString());

        private static void Toaster(string message)
            => Device.BeginInvokeOnMainThread(()
                => DependencyService.Get<INotifier>().MakeToast(message));
    }
}
