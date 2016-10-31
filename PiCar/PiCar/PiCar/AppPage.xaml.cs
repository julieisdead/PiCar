using System;
using System.Threading.Tasks;
using MqttLib;
using PiCar.Droid;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;

namespace PiCar
{
    public partial class AppPage : ContentPage
    {
        private Movement movement;

        private IMqtt client;

        public AppPage()
        {
            InitializeComponent();
            Title = "PiCar";
            CrossConnectivity.Current.ConnectivityChanged += ConnectivityChanged;
            CrossConnectivity.Current.ConnectivityTypeChanged += ConnectivityChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ConnectToServer();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            movement = new Movement();
            SendToMosquitto(movement.ToString());
        }

        private void SettingsClicked(object sender, EventArgs e) => ShowSettingsPage();

        private void OnForwardTouched(object sender, EventArgs e)
        {
            if (movement.Reverse) return;
            movement.Forward = ((CarButton)sender).State;
            MoveCar();
        }

        private void OnReverseTouched(object sender, EventArgs e)
        {
            if (movement.Forward) return;
            movement.Reverse = ((CarButton)sender).State;
            MoveCar();
        }

        private void OnLeftTouched(object sender, EventArgs e)
        {
            if (movement.Right) return;
            movement.Left = ((CarButton)sender).State;
            MoveCar();
        }

        private void OnRightTouched(object sender, EventArgs e)
        {
            if (movement.Left) return;
            movement.Right = ((CarButton) sender).State;
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
                    MainLayout.Children.Add(CarCamView,
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
                    ConnectToServer();
                }
                else
                {
                    NavigationPage.SetHasNavigationBar(this, true);
                    MainLayout.Children.Clear();

                    // Car Cam View
                    MainLayout.Children.Add(CarCamView,
                        Constraint.RelativeToParent((parent) => parent.X),
                        Constraint.RelativeToParent((parent) => parent.Y),
                        Constraint.RelativeToParent((parent) => parent.Width),
                        Constraint.RelativeToParent((parent) => parent.Height*0.5));

                    // StatusText
                    MainLayout.Children.Add(StatusText,
                        Constraint.RelativeToParent((parent) => parent.X + 20),
                        Constraint.RelativeToParent((parent) => parent.Height*0.5 - StatusText.Height*1.5),
                        Constraint.RelativeToParent((parent) => parent.Width),
                        Constraint.Constant(StatusText.Height));

                    MainLayout.Children.Add(ForwardButton,
                        Constraint.RelativeToParent((parent) => parent.X + 20),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.5 + 20),
                        Constraint.Constant(75),
                        Constraint.Constant(75));

                    MainLayout.Children.Add(ReverseButton,
                        Constraint.RelativeToParent((parent) => parent.X + 60),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.5 + 115),
                        Constraint.Constant(75),
                        Constraint.Constant(75));

                    MainLayout.Children.Add(RightButton,
                        Constraint.RelativeToParent((parent) => parent.Width - 95),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.5 + 20),
                        Constraint.Constant(75),
                        Constraint.Constant(75));

                    MainLayout.Children.Add(LeftButton,
                        Constraint.RelativeToParent((parent) => parent.Width - 135),
                        Constraint.RelativeToParent((parent) => parent.Y + parent.Height*0.5 + 115),
                        Constraint.Constant(75),
                        Constraint.Constant(75));
                    ConnectToServer();
                }
            }
        }

        public void ConnectToServer()
        {
            Settings settings = Settings.LoadSettings();
            IWifi wifi = new Wifi();

            string server;
            if (wifi.GetSSID() == $"\"{settings.LocalSSID}\"" && !string.IsNullOrEmpty(settings.LocalSSID))
                server = settings.LocalServerName;
            else server = settings.RemoteServerName;

            if (string.IsNullOrEmpty(server))
                ShowSettingsPage();

            StatusText.Text = "Connecting...";

            string html = "<html><head><style>" +
                          $"body {{ margin: 0px; padding: 0px; background-color: #263238; Width: {CarCamView.Width}px; Height: {CarCamView.Height}px;}} " +
                          "img  { width: 100%; } </style> </head><body>" +
                          $"<img src=\"http://{server}:{settings.CameraPort}/test.mjpg\" onerror=\"this.src = 'not-connected.png'\" />" +
                          "</body></html>";

            CarCamView.LoadContent(html, DependencyService.Get<IBaseUrl>().Get());
            string connectionString = $"tcp://{server}:{settings.MqttPort}";
            ConnectMqtt(connectionString, settings.Username, settings.Password);
        }

        public void ConnectMqtt(string connectionString, string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    client = MqttClientFactory.CreateClient(connectionString,
                        Guid.NewGuid().ToString());
                }
                else
                {
                    client = MqttClientFactory.CreateClient(connectionString,
                        Guid.NewGuid().ToString(), username , password);
                }
                client.Connected += client_Connected;
                client.ConnectionLost += client_ConnectionLost;
                client.PublishArrived += client_PublishArrived;
                Task.Run(() => client.Connect(true));
            }
            catch
            {
                //
            }
        }

        private void ConnectivityChanged(object sender, ConnectivityChangedEventArgs args) => ConnectToServer();

        private void ConnectivityChanged(object sender, ConnectivityTypeChangedEventArgs args) => ConnectToServer();

        public void SendToMosquitto(string value)
        {
            try
            {
                MqttPayload payload = new MqttPayload(value);
                client.Publish("car/REQUEST", payload, QoS.BestEfforts, false);
            }
            catch
            {
                //
            }
        }

        private void client_Connected(object sender, EventArgs e) => Device.BeginInvokeOnMainThread(() =>
        {
            StatusText.Text = "Mqtt client connected.";
            RegisterOurSubscriptions();

            movement = new Movement();
            SendToMosquitto(movement.ToString());
        });

        private void client_ConnectionLost(object sender, EventArgs e)
            => Device.BeginInvokeOnMainThread(() => StatusText.Text = "Client connection lost.");

        private void RegisterOurSubscriptions()
        {
            try
            {
                client.Subscribe("car/RESPOND", QoS.BestEfforts);
            }
            catch
            {
                StatusText.Text = "Not Connected to a broker";
            }
        }

        private bool client_PublishArrived(object sender, PublishArrivedArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                string response = e.Payload.ToString();
                StatusText.Text = response;
            });
            return true;
        }

        private void ShowSettingsPage()
        {
            if (Settings.IsOpen) return;
            Navigation.PushAsync(new SettingsPage(), true);
        }

        private void MoveCar() => SendToMosquitto(movement.ToString());
    }
}
