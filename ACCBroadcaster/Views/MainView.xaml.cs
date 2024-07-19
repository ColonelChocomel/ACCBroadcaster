using ACCBroadcaster.Classes;
using ACCBroadcaster.Views.Broadcasting;
using ksBroadcastingNetwork;
using ksBroadcastingNetwork.Structs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ACCBroadcaster.Properties;
using System.Reflection;
using System.Diagnostics;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ACCBroadcaster.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        private ObservableCollection<Theme> themeOptions = new ObservableCollection<Theme>();

        public MainView()
        {
            themeOptions.Add(new Theme("Default", null));
            themeOptions.Add(new Theme("Light Mode", "light"));
            themeOptions.Add(new Theme("Dark Mode", "dark"));

            this.InitializeComponent();

            if(Settings.Default.Theme == "light")
            {
                SelectedTheme.SelectedIndex = 1;
            }else if(Settings.Default.Theme == "dark")
            {
                SelectedTheme.SelectedIndex = 2;
            }
            else
            {
                SelectedTheme.SelectedIndex = 0;
            }

            if (Settings.Default.LoginRemembered)
            {
                IP.Text = Settings.Default.IP;
                Port.Value = Settings.Default.Port;
                DisplayName.Text = Settings.Default.DisplayName;
                ConnectionPW.Password = Settings.Default.ConnectionPw;
                CommandPW.Password = Settings.Default.CommandPw;
                UpdateInterval.Value = Settings.Default.UpdateInterval;
                RememberLogin.IsChecked = true;
            }
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionText.Text = $"Version {version.Major}.{version.Minor}.{version.Build}";
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = IP.Text;
            int port = (int)Port.Value;
            string displayName = DisplayName.Text;
            string connectionPw = ConnectionPW.Password;
            string commandPw = CommandPW.Password;
            int updateInterval = (int)UpdateInterval.Value;
            if ((bool)RememberLogin.IsChecked)
            {
                Settings.Default.IP = ip;
                Settings.Default.Port = port;
                Settings.Default.DisplayName = displayName;
                Settings.Default.ConnectionPw = connectionPw;
                Settings.Default.CommandPw = commandPw;
                Settings.Default.UpdateInterval = updateInterval;
                Settings.Default.LoginRemembered = true;
                Settings.Default.Save();
            }
            ACCService.Client = new ACCUdpRemoteClient(ip, port, displayName, connectionPw, commandPw, updateInterval);
            ACCService.Client.MessageHandler.OnConnectionStateChanged += OnConnect;
            ACCService.Client.MessageHandler.OnDisconnect += OnDisconnect;
        }

        private void ThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Theme theme = e.AddedItems[0] as Theme;
            
            if (theme.Value == "dark")
            {
                this.RequestedTheme = ElementTheme.Dark;
            }
            else if (theme.Value == "light")
            {
                this.RequestedTheme = ElementTheme.Light;
            }
            else
            {
                this.RequestedTheme = ElementTheme.Default;
            }

            Settings.Default.Theme = theme.Value;
            Settings.Default.Save();
        }

        private void OnConnect(int connectionId, bool connectionSuccess, bool isReadonly, string error)
        {
            if (connectionSuccess)
            {
                Frame.Navigate(typeof(BroadcastingView));
            } else
            {
                ErrorTextBlock.Text = error;
            }
        }

        private void OnDisconnect()
        {
            Frame.Navigate(typeof(MainView));
        }
    }
}
