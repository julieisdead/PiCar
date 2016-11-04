using System;
using System.Threading;
using System.Threading.Tasks;
using MqttLib;
using PiCar.Droid;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;
using Random = System.Random;

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

            movement = new Movement();
            CrossConnectivity.Current.ConnectivityChanged += ConnectivityChanged;
            CrossConnectivity.Current.ConnectivityTypeChanged += ConnectivityChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StartConnectToAll();
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
            movement.Forward = ((CarButton)sender).State == CarButtonState.Down;
            MoveCar();
        }

        private void OnReverseTouched(object sender, EventArgs e)
        {
            if (movement.Forward) return;
            movement.Reverse = ((CarButton)sender).State == CarButtonState.Down;
            MoveCar();
        }

        private void OnLeftTouched(object sender, EventArgs e)
        {
            if (movement.Right) return;
            movement.Left = ((CarButton)sender).State == CarButtonState.Down;
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
                  //  tokenSource.Cancel();
                    StartConnectToAll();
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
                    //tokenSource.Cancel();
                    StartConnectToAll();
                }
            }
        }

        private static bool connecting;

        private void StartConnectToAll()
        {
            if (connecting) return;
            connecting = true;
            bool cam = ConnectToCam();
            bool mqtt = ConnectToBroker();
            Toaster("Connecting...");
            connecting = !(mqtt && cam);
            if(!connecting) Toaster("Connected");
            if (!connecting) return;

            Device.StartTimer(TimeSpan.FromMilliseconds(10000), () =>
            {
                if (Settings.IsOpen) return true;
                cam = ConnectToCam();
                mqtt = ConnectToBroker();
                Toaster("Connecting...");
                System.Diagnostics.Debug.WriteLine("Lopper");
                connecting = !(mqtt && cam);
                if(!connecting) Toaster("Connected.");
                return connecting;
            });
        }

        private bool ConnectToCam()
        {
            Settings settings = Settings.LoadSettings();
            IWifi wifi = new Wifi();

            string server;
            if (wifi.GetSSID() == $"\"{settings.LocalSSID}\"" && !string.IsNullOrEmpty(settings.LocalSSID))
                server = settings.LocalServerName;
            else server = settings.RemoteServerName;

            if (string.IsNullOrWhiteSpace(server))
            {
                ShowSettingsPage();
                return true;
            }

            bool camConnected = CrossConnectivity.Current.IsRemoteReachable(server, settings.CameraPort, 1250);

            string html = "<html><head><style>" +
              $"body {{ margin: 0px; padding: 0px; background-color: #263238; Width: {CarCamView.Width}px; Height: {CarCamView.Height}px;}} " +
              "img  { width: 100%; } </style> </head><body>" +
              $"<img src=\"http://{server}:{settings.CameraPort}/test.mjpg\" onerror=\"this.src = ''\" />" +
              "</body></html>";

            if(camConnected)
                CarCamView.LoadContent(html, DependencyService.Get<IBaseUrl>().Get());

            return camConnected;
        }

        private bool ConnectToBroker()
        {
            Settings settings = Settings.LoadSettings();
            IWifi wifi = new Wifi();

            string server;
            if (wifi.GetSSID() == $"\"{settings.LocalSSID}\"" && !string.IsNullOrEmpty(settings.LocalSSID))
                server = settings.LocalServerName;
            else server = settings.RemoteServerName;

            if (string.IsNullOrEmpty(server))
            {
                ShowSettingsPage();
                return true;
            }

            bool mqttConnected = CrossConnectivity.Current.IsRemoteReachable(server, settings.CameraPort, 1250);

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
                Task.Run(() => client.Connect(true));
            }
            catch
            {
                return false;
            }
            return mqttConnected;
        }

        private void ConnectivityChanged(object sender, ConnectivityChangedEventArgs args)
        {
            StartConnectToAll();
        }

        private void ConnectivityChanged(object sender, ConnectivityTypeChangedEventArgs args)
        {
            StartConnectToAll();
        }

        private void SendToMosquitto(string value)
        {
            try
            {
                MqttPayload payload = new MqttPayload(value);
                client.Publish("car/REQUEST", payload, QoS.BestEfforts, false);
            }
            catch
            {
                StartConnectToAll();
            }
        }

        private void client_Connected(object sender, EventArgs e) => Device.BeginInvokeOnMainThread(() =>
        {
            RegisterOurSubscriptions();

            movement = new Movement();
            SendToMosquitto(movement.ToString());
        });

        private void client_ConnectionLost(object sender, EventArgs e)
        {
            Toaster("Client connection lost.");
            StartConnectToAll();
        }

        private void RegisterOurSubscriptions()
        {
            try
            {
                client.Subscribe("car/RESPOND", QoS.BestEfforts);
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

        private static void Toaster(string message)
            => Device.BeginInvokeOnMainThread(()
                => DependencyService.Get<INotifier>().MakeToast(message));
    }
}
