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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ACCBroadcaster.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        public MainView()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = IP.Text;
            int port = (int)Port.Value;
            string displayName = DisplayName.Text;
            string connectionPw = ConnectionPW.Password;
            string commandPw = CommandPW.Password;
            int updateInterval = (int)UpdateInterval.Value;
            ACCService.client = new ACCUdpRemoteClient(ip, port, displayName, connectionPw, commandPw, updateInterval);
            ACCService.client.MessageHandler.OnTrackDataUpdate += OnConnect;
        }

        private void OnConnect(string sender, TrackData trackUpdate)
        {
            Frame.Navigate(typeof(BroadcastingView));
        }
    }
}
