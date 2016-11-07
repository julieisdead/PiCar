using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MqttLib;
using PiCar.Droid;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;

namespace PiCar
{
    public partial class AppPage : ContentPage
    {
        private Movement movement;
        private IMqtt client;
        private ICommand refreshCommand;

        public ICommand RefreshCommand
            => refreshCommand ?? (refreshCommand = new Command(async ()
                => await ExecuteRefreshCommand()));

        public AppPage()
        {
            InitializeComponent();
            Title = "PiCar";

            movement = new Movement();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Connect();
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
        protected override async void OnSizeAllocated(double newWidth, double newHeight)
        {
            base.OnSizeAllocated(newWidth, newHeight);
            if (Math.Abs(newWidth - pageWidth) > 0 || Math.Abs(newHeight - pageHeight) > 0)
            {
                pageWidth = newWidth;
                pageHeight = newHeight;
                CamPullLayout.RefreshCommand = RefreshCommand;

                if (newWidth > newHeight)
                {
                    NavigationPage.SetHasNavigationBar(this, false);
                    MainLayout.Children.Clear();

                    CamPullLayout = new PullToRefreshLayout
                    {
                        Content = CamScrollView,
                        RefreshCommand = RefreshCommand
                    };

                    //Car Cam View
                    MainLayout.Children.Add(CamPullLayout,
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
                    await Connect();
                }
                else
                {
                    NavigationPage.SetHasNavigationBar(this, true);
                    MainLayout.Children.Clear();

                    CamPullLayout = new PullToRefreshLayout
                    {
                        Content = CamScrollView,
                        RefreshCommand = RefreshCommand
                    };

                    // Car Cam View
                    MainLayout.Children.Add(CamPullLayout,
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
                    await Connect();
                }
            }
        }

        private async Task ExecuteRefreshCommand()
        {
            if (CamPullLayout.IsRefreshing)
                return;
            CamPullLayout.IsRefreshing = true;
            await Connect();
            CamPullLayout.IsRefreshing = false;
        }

        private async Task Connect()
        {
            ConnectToBroker();
            await Task.Delay(TimeSpan.FromMilliseconds(1000));
            ConnectToCam();
        }

        private void ConnectToCam()
        {
            if (client == null) return;
            if (!client.IsConnected) return;

            Settings settings = Settings.LoadSettings();
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
                $"body {{ margin: 0px; padding: 0px; background-color: transparent; Width: {CamWebView.Width}px; Height: {CamWebView.Height}px;}} " +
                "img  { width: 100%; position: absolute; top 0; left 0; } </style> </head><body>" +
                $"<img src=\"http://{server}:{settings.CameraPort}/test.mjpg\" onerror=\"this.src = '';\" />" +
                "</body></html>";

            CamWebView.LoadContent(html, DependencyService.Get<IBaseUrl>().Get());
        }

        private void ConnectToBroker()
        {
            Settings settings = Settings.LoadSettings();
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
                Task.Run(() => client.Connect(true));
            }
            catch
            {
                //
            }
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
                //
            }
        }

        private void client_Connected(object sender, EventArgs e) => Device.BeginInvokeOnMainThread(() =>
        {
            RegisterOurSubscriptions();

            movement = new Movement();
            SendToMosquitto(movement.ToString());
        });

        private static void client_ConnectionLost(object sender, EventArgs e) => Toaster("Client connection lost.");

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
            Settings.IsOpen = true;
            Navigation.PushAsync(new SettingsPage(), true);
        }

        private void MoveCar() => SendToMosquitto(movement.ToString());

        private static void Toaster(string message)
            => Device.BeginInvokeOnMainThread(()
                => DependencyService.Get<INotifier>().MakeToast(message));
    }
}
