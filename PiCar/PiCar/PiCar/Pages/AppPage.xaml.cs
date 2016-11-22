using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using PiCar.Droid;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xamarin.Forms;

namespace PiCar
{
    public partial class AppPage : ContentPage
    {
        private Movement movement;
        private static MqttClient client;
        private static ISettings AppSettings => CrossSettings.Current;
        private const string SettingsKey = "A68d709c745c446cace320c5ec07556f";
        public static string SelectedServer;

        public AppPage()
        {
            InitializeComponent();
            Title = "PiCar";
            SelectedServer = string.Empty;
            BackgroundColor = Color.FromHex("#CFD8DC");
            movement = new Movement();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            List<string> servers = Settings.GetServers();
            Servers.Items.Clear();
            foreach (string item in servers)
                Servers.Items.Add(item);

            if (Servers.Items.Count == 0)
            {
                ShowSettingsPage();
                return;
            }

            if (string.IsNullOrEmpty(SelectedServer))
                SelectedServer = AppSettings.GetValueOrDefault(SettingsKey, string.Empty);

            Servers.SelectedIndex = Servers.Items.IndexOf(SelectedServer);

            if (Servers.SelectedIndex < 0)
                Servers.SelectedIndex = 0;

            AppSettings.AddOrUpdateValue(SettingsKey, Servers.Items[Servers.SelectedIndex]);

            Connect();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            movement = new Movement();
            SendToMosquitto(movement.ToString());
            if (client != null && client.IsConnected)
                client.Disconnect();
            CamWebView.LoadContent("");
        }

        private void EditClicked(object sender, EventArgs e) => ShowSettingsPage();

        private void RefreshClicked(object sender, EventArgs e) => Connect();

        private void ServersChanged(object sender, EventArgs e)
        {
            if (Servers.SelectedIndex >= 0)
            {
                AppSettings.AddOrUpdateValue(SettingsKey, Servers.Items[Servers.SelectedIndex]);
                Connect();
            }
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

            client = new MqttClient(server, settings.MqttPort, false, null, null, MqttSslProtocols.None);

            string username = string.Empty;
            string password = string.Empty;
            if (!string.IsNullOrEmpty(settings.Username))
            {
                username = settings.Username;
                password = settings.Password;
            }

            client.ConnectionClosed += client_ConnectionLost;
            client.MqttMsgPublishReceived += client_PublishArrived;

            try
            {
                client.Connect(Guid.NewGuid().ToString(), username, password, true,
                    MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                    true, "car/DISCONNECT", XLabs.Platform.Device.AndroidDevice.CurrentDevice.Name, true, 30);
            }
            catch
            {
                Toaster("Failed to connect to broker.");
                return;
            }
            if (!client.IsConnected)
            {
                Toaster("Failed to connect to broker.");
                return;
            }

            RegisterOurSubscriptions();
            client.Publish("car/CONNECT",
                Encoding.Default.GetBytes(XLabs.Platform.Device.AndroidDevice.CurrentDevice.Name),
                MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            movement = new Movement();
            SendToMosquitto(movement.ToString());
        }

        private static void SendToMosquitto(string value)
        {
            try
            {
                if(!client.IsConnected) return;
                byte[] payload = Encoding.Default.GetBytes(value);
                client.Publish("car/REQUEST", payload, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            }
            catch
            {
                //
            }
        }

        private void client_ConnectionLost(object sender, EventArgs e)
        {
            Toaster("Client disconnected");
        }

        private static void RegisterOurSubscriptions()
        {
            try
            {
                client.Subscribe(new[] {"car/RESPOND"}, new[] {MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE});
                client.Subscribe(new[] {"car/STATE"}, new[] {MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE});
            }
            catch
            {
                //
            }
        }

        private void client_PublishArrived(object sender, MqttMsgPublishEventArgs e) => Device.BeginInvokeOnMainThread(() =>
        {
            string response = Encoding.Default.GetString(e.Message);
            switch (e.Topic)
            {
                case "car/STATE":
                    StatusText.Text = response;
                    return;
                case "car/RESPOND":
                    Toaster(response);
                    break;
            }
        });

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
