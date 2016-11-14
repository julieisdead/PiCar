using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PiCar
{
    public class Server
    {
        private const string SettingsKey = "4db3669d-2162-4eb1-86a9-20a063997745";

        public Server()
        {
            _remoteServer = string.Empty;
            _localSSID = string.Empty;
            _localServer = string.Empty;
            _username = string.Empty;
            _password = string.Empty;
            _mqttPort = 1883;
            _cameraPort = 8081;
        }

        public static Server LoadSettings()
        {
            string settings = AppSettings.GetValueOrDefault(SettingsKey, new Server().ToString());
            return JsonConvert.DeserializeObject<Server>(settings);
        }

        public static bool IsOpen { get; set; }

        public string RemoteServerName
        {
            get { return _remoteServer; }
            set
            {
                _remoteServer = value;
                SaveSettings();
            }
        }
        public string LocalSSID
        {
            get { return _localSSID; }
            set
            {
                _localSSID = value;
                SaveSettings();
            }
        }
        public string LocalServerName
        {
            get { return _localServer; }
            set
            {
                _localServer = value;
                SaveSettings();
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                SaveSettings();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                SaveSettings();
            }
        }

        public int MqttPort
        {
            get { return _mqttPort; }
            set
            {
                _mqttPort = value;
                SaveSettings();
            }
        }
        public int CameraPort
        {
            get { return _cameraPort; }
            set
            {
                _cameraPort = value;
                SaveSettings();
            }
        }

        private string _remoteServer;
        private string _localSSID;
        private string _localServer;
        private string _username;
        private string _password;
        private int _mqttPort;
        private int _cameraPort;

        private static ISettings AppSettings => CrossSettings.Current;

        private void SaveSettings()
        {
            string settings = ToString();
            AppSettings.AddOrUpdateValue(SettingsKey, settings);
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}