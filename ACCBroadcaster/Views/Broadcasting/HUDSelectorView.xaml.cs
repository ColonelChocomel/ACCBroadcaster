using ACCBroadcaster.Classes;
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ACCBroadcaster.Views.Broadcasting
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HUDSelectorView : Page
    {
        private ObservableCollection<HUDOption> hudOptions = new ObservableCollection<HUDOption>();

        public HUDSelectorView()
        {
            hudOptions.Add(new HUDOption("Blank", "Blank"));
            hudOptions.Add(new HUDOption("Basic HUD", "Basic HUD"));
            hudOptions.Add(new HUDOption("Help", "Help"));
            hudOptions.Add(new HUDOption("Time Table", "TimeTable"));
            hudOptions.Add(new HUDOption("Broadcasting", "Broadcasting"));
            hudOptions.Add(new HUDOption("Track Map", "TrackMap"));
            ACCService.Client.MessageHandler.OnRealtimeUpdate += OnRealtimeUpdate;
            this.InitializeComponent();
        }

        void OnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            HUDOption hudPage = hudOptions.FirstOrDefault(x => x.InternalName == button.Name);
            if (hudPage != null)
            {
                ACCService.Client.MessageHandler.RequestHUDPage(hudPage.InternalName);
            }
        }

        private void OnRealtimeUpdate(string sender, RealtimeUpdate update)
        {
            HUDOption hudPage = hudOptions.FirstOrDefault(x => x.InternalName == update.CurrentHudPage);
            if (hudPage != null)
            {
                HUDOption currentHudPage = hudOptions.FirstOrDefault(x => x.IsActive == true);
                if (currentHudPage == null)
                {
                    hudPage.SetActive(true);
                }
                else if (currentHudPage != hudPage)
                {
                    hudPage.SetActive(true);
                    currentHudPage.SetActive(false);
                }
            }
        }
    }
}
