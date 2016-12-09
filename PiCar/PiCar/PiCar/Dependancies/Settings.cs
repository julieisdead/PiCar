using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PiCar
{
    public class Settings : INotifyPropertyChanged
    {
        private const string SettingsKey = "4db3669d21624eb186a920a063997745";
        private static Dictionary<string, Settings> servers;
        private static ISettings AppSettings => CrossSettings.Current;

        public Settings()
        {
            ServerKey = string.Empty;
            Name = string.Empty;
            RemoteServerName = string.Empty;
            EnableControls = false;
            LocalSSID = string.Empty;
            LocalServerName = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            MqttPort = 1883;
            CameraPort = 8081;
        }

        public void LoadServer(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            servers = new Dictionary<string, Settings>();
            string settings = AppSettings.GetValueOrDefault(SettingsKey, string.Empty);
            if (settings == string.Empty) return;
            servers = JsonConvert.DeserializeObject<Dictionary<string, Settings>>(settings);
            ServerKey = servers[name].ServerKey;
            Name = servers[name].Name;
            RemoteServerName = servers[name].RemoteServerName;
            EnableControls = servers[name].EnableControls;
            LocalSSID = servers[name].LocalSSID;
            LocalServerName = servers[name].LocalServerName;
            Username = servers[name].Username;
            Password = servers[name].Password;
            MqttPort = servers[name].MqttPort;
            CameraPort = servers[name].CameraPort;
        }

        public static Settings LoadSettings(string serverKey)
        {
            if(string.IsNullOrEmpty(serverKey)) return new Settings();
            servers = new Dictionary<string, Settings>();
            string settings = AppSettings.GetValueOrDefault(SettingsKey, string.Empty);
            if(settings == string.Empty) return new Settings();
            servers = JsonConvert.DeserializeObject<Dictionary<string, Settings>>(settings);
            return servers.ContainsKey(serverKey) ? servers[serverKey] : new Settings();
        }

        public static List<string> GetServers()
        {
            servers = new Dictionary<string, Settings>();
            string settings = AppSettings.GetValueOrDefault(SettingsKey, string.Empty);
            if (settings == string.Empty) return new List<string>();
            servers = JsonConvert.DeserializeObject<Dictionary<string, Settings>>(settings);
            List<string> keys = servers.Select(item => item.Key).ToList();
            keys.Sort();
            return keys;
        }

        public void SaveServer()
        {
            if (string.IsNullOrEmpty(Name)) return;

            if (servers.ContainsKey(ServerKey))
                servers.Remove(ServerKey);

            if (servers.ContainsKey(Name))
                servers.Remove(Name);

            ServerKey = Name;
            servers.Add(ServerKey, this);

            AppPage.SelectedServer = ServerKey;
            string settings = JsonConvert.SerializeObject(servers);
            AppSettings.AddOrUpdateValue(SettingsKey, settings);
        }

        public void DeleteServer()
        {
            if (servers.ContainsKey(ServerKey))
            {
                servers.Remove(ServerKey);
            }
            string settings = JsonConvert.SerializeObject(servers);
            AppPage.SelectedServer = string.Empty;
            AppSettings.AddOrUpdateValue(SettingsKey, settings);
        }

        public void AddServer()
        {
            ServerKey = string.Empty;
            Name = string.Empty;
            RemoteServerName = string.Empty;
            EnableControls = false;
            LocalSSID = string.Empty;
            LocalServerName = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            MqttPort = 1883;
            CameraPort = 8081;
            AppPage.SelectedServer = string.Empty;
        }

        public static bool IsOpen { get; set; }
        public string ServerKey { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string RemoteServerName
        {
            get { return _remoteServerName; }
            set
            {
                if (_remoteServerName != value)
                {
                    _remoteServerName = value;
                    OnPropertyChanged(nameof(RemoteServerName));
                }
            }
        }

        public string LocalSSID
        {
            get { return _localSsid; }
            set
            {
                if (_localSsid != value)
                {
                    _localSsid = value;
                    OnPropertyChanged(nameof(LocalSSID));
                }
            }
        }
        public string LocalServerName
        {
            get { return _localServerName; }
            set
            {
                if (_localServerName != value)
                {
                    _localServerName = value;
                    OnPropertyChanged(nameof(LocalServerName));
                }
            }
        }

        public bool EnableControls
        {
            get { return _enableControls; }
            set
            {
                if (_enableControls != value)
                {
                    _enableControls = value;
                    OnPropertyChanged(nameof(EnableControls));
                }
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
        public int MqttPort
        {
            get { return _mqttPort; }
            set
            {
                if (_mqttPort != value)
                {
                    _mqttPort = value;
                    OnPropertyChanged(nameof(MqttPort));
                }
            }
        }
        public int CameraPort
        {
            get { return _cameraPort; }
            set
            {
                if (_cameraPort != value)
                {
                    _cameraPort = value;
                    OnPropertyChanged(nameof(CameraPort));
                }
            }
        }

        private string _name;
        private string _remoteServerName;
        private string _localSsid;
        private string _localServerName;
        private bool _enableControls;
        private string _username;
        private string _password;
        private int _mqttPort;
        private int _cameraPort;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}