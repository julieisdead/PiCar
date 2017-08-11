using System;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Widget;
using MqttLib;
using PiCar.Droid;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;

namespace PiCar
{
    public partial class AppPage : ContentPage
    {
        private Movement movement;
        public static IMqtt client;
        private static ISettings AppSettings => CrossSettings.Current;
        private const string SettingsKey = "A68d709c745c446cace320c5ec07556f";
        public static string SelectedServer;
        private static bool _clientConnected;

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

            DependencyService.Get<IOrientate>().SetOrientation(ScreenOrientation.User);

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
            DrawScreen();
            Connect();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            movement = new Movement();
            SendToMosquitto(movement.ToString());
            Disconnect();
        }

        private void EditClicked(object sender, EventArgs e) => ShowSettingsPage();

        private void RefreshClicked(object sender, EventArgs e)
        {
            Disconnect();
            Connect();
        }

        private void ServersChanged(object sender, EventArgs e)
        {
            if (Servers.SelectedIndex >= 0)
            {
                SelectedServer = Servers.Items[Servers.SelectedIndex];
                AppSettings.AddOrUpdateValue(SettingsKey, SelectedServer);
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
        private Orientation currentOrientation;

        protected override void OnSizeAllocated(double newWidth, double newHeight)
        {
            base.OnSizeAllocated(newWidth, newHeight);
            if (Math.Abs(newWidth - pageWidth) > 0 || Math.Abs(newHeight - pageHeight) > 0)
            {
                pageWidth = newWidth;
                pageHeight = newHeight;
                currentOrientation = newWidth > newHeight
                    ? Orientation.Horizontal
                    : Orientation.Vertical;
                DrawScreen();
            }
        }


        private void DrawScreen()
        {
            Settings settings = new Settings();
            if (Servers.Items.Contains(SelectedServer))
            {
                settings = Settings.LoadSettings(SelectedServer);
            }

            if(currentOrientation == Orientation.Horizontal)
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
                    Constraint.RelativeToParent((parent) => CamWebView.Height - 50),
                    Constraint.RelativeToParent((parent) => parent.Width*0.7),
                    Constraint.Constant(24.7f));

                MainLayout.Children.Add(EditButton,
                    Constraint.RelativeToParent((parent) => parent.Width - 60),
                    Constraint.RelativeToParent((parent) => 0));
                EditButton.TextColor = Color.Gray;

                MainLayout.Children.Add(RefreshButton,
                    Constraint.RelativeToParent((parent) => parent.Width - 105),
                    Constraint.RelativeToParent((parent) => 0));
                RefreshButton.TextColor = Color.Gray;

                if (settings.EnableControls)
                {
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
                    Constraint.RelativeToParent((parent) => parent.Height*0.5 - 25),
                    Constraint.RelativeToParent((parent) => parent.Width*0.5));

                FakeToolbar.Children.Add(EditButton,
                    Constraint.RelativeToParent((parent) => parent.Width - 60),
                    Constraint.RelativeToParent((parent) => parent.Height*0.5 - 25));
                EditButton.TextColor = Color.FromHex("#ECEFF1");

                FakeToolbar.Children.Add(RefreshButton,
                    Constraint.RelativeToParent((parent) => parent.Width - 105),
                    Constraint.RelativeToParent((parent) => parent.Height*0.5 - 25));
                RefreshButton.TextColor = Color.FromHex("#ECEFF1");

                // Car Cam View
                MainLayout.Children.Add(CamWebView,
                    Constraint.RelativeToParent((parent) => parent.X),
                    Constraint.RelativeToParent((parent) => parent.Y + 50),
                    Constraint.RelativeToParent((parent) => parent.Width),
                    Constraint.RelativeToParent((parent) => parent.Height*0.5));

                // StatusText
                MainLayout.Children.Add(StatusText,
                    Constraint.RelativeToParent((parent) => parent.X + 20),
                    Constraint.RelativeToParent((parent) => CamWebView.Height),
                    Constraint.RelativeToParent((parent) => parent.Width),
                    Constraint.Constant(24.7f));

                if (settings.EnableControls)
                {
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
            }
            ConnectToCam();
        }

        public void Connect()
        {
            ConnectToBroker();
            ConnectToCam();
        }

        public void Disconnect()
        {
            try
            {
                movement = new Movement();
                MoveCar();
                if (client != null && client.IsConnected)
                    client.Disconnect();
                CamWebView.LoadContent("");
            }
            catch (Exception ex)
            {
                Toaster(ex.Message, ToastPriority.Critical, ToastLength.Long);
            }
            finally
            {
                _clientConnected = false;
            }
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
                          $"<img class=\"camview\" src=\"http://{server}:{settings.CameraPort}/?action=stream\" onerror=\"this.src = '';\" />" +
                          "</body></html>";

            Device.BeginInvokeOnMainThread(() => CamWebView.LoadContent(html, DependencyService.Get<IBaseUrl>().Get()));
        }

        private void ConnectToBroker()
        {
            if (Servers.SelectedIndex < 0)
            {
                ShowSettingsPage();
                return;
            }
            if (_clientConnected) return;

            Settings settings = Settings.LoadSettings(Servers.Items[Servers.SelectedIndex]);
            if (!settings.EnableControls) return;
            IWifi wifi = new Wifi();

            string server = wifi.GetSSID() == $"\"{settings.LocalSSID}\""
                ? settings.LocalServerName
                : settings.RemoteServerName;

            string connectionString = $"tcp://{server}:{settings.MqttPort}";

            Device.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrEmpty(settings.Username))
                    client = MqttClientFactory.CreateClient(connectionString, Guid.NewGuid().ToString(), settings.Username,
                        settings.Password);
                else
                    client = MqttClientFactory.CreateClient(connectionString, Guid.NewGuid().ToString());

                client.Connected += ClientConnected;
                client.ConnectionLost += ClientConnectionLost;
                client.PublishArrived += ClientPublishArrived;

                IDeviceInfo device = new DeviceInfo();
                string name = device.GetName();

                try
                {
                    client.Connect("car/DISCONNECT", QoS.BestEfforts, new MqttPayload(name), false, true);
                }
                catch (Exception ex)
                {
                    Toaster(ex.Message, ToastPriority.Critical, ToastLength.Long);
                    _clientConnected = false;
                }
            });
        }

        private static void SendToMosquitto(string value)
        {
            try
            {
                if(!_clientConnected) return;
                client.Publish("car/REQUEST", new MqttPayload(value), QoS.AtLeastOnce, false);
            }
            catch(Exception ex)
            {
                Toaster(ex.Message, ToastPriority.Critical, ToastLength.Long);
            }
        }

        private void ClientConnected(object sender, EventArgs e) => Device.BeginInvokeOnMainThread(delegate
        {
            client.Subscribe("car/STATE", QoS.BestEfforts);
            client.Subscribe("car/RESPOND", QoS.BestEfforts);
            client.Subscribe("car/STATUS", QoS.BestEfforts);
            StatusText.Text = string.Empty;

            _clientConnected = true;

            IDeviceInfo device = new DeviceInfo();
            string name = device.GetName();
            MqttPayload payload = new MqttPayload(name);
            client.Publish("car/CONNECT", payload, QoS.BestEfforts, false);
            movement = new Movement();
            MoveCar();
        });


        private void ClientConnectionLost(object sender, EventArgs e) => Device.BeginInvokeOnMainThread(() =>
        {
            _clientConnected = false;
            StatusText.Text = string.Empty;
            Toaster("Client connection lost.");
        });

        private bool ClientPublishArrived(object sender, PublishArrivedArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                switch (e.Topic)
                {
                    case "car/STATE":
                        StatusText.Text = e.Payload.ToString();
                        return;
                    case "car/RESPOND":
                        Toaster(e.Payload.ToString());
                        break;
                    case "car/STATUS":
                        Toaster(e.Payload.ToString(), ToastPriority.High);
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
                => DependencyService.Get<INotifier>().MakeToast(message, ToastPriority.Low, ToastLength.Short));
        /**
        //Not used yet.
        private static void Toaster(string message, ToastLength length)
            => Device.BeginInvokeOnMainThread(()
                => DependencyService.Get<INotifier>().MakeToast(message, ToastPriority.Low, length));
        /**/

        private static void Toaster(string message, ToastPriority priority)
            => Device.BeginInvokeOnMainThread(()
                => DependencyService.Get<INotifier>().MakeToast(message, priority, ToastLength.Short));
        private static void Toaster(string message, ToastPriority priority, ToastLength length)
            => Device.BeginInvokeOnMainThread(()
                => DependencyService.Get<INotifier>().MakeToast(message, priority, length));
    }
}
