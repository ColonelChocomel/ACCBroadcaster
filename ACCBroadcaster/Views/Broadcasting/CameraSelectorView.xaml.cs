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
    public sealed partial class CameraSelectorView : Page
    {
        private ObservableCollection<Camera> Cameras = new ObservableCollection<Camera>();
        private ObservableCollection<Camera> OnboardCameras = new ObservableCollection<Camera>();
        private ObservableCollection<Camera> DrivableCameras = new ObservableCollection<Camera>();
        private bool OnboardIsActive = false;
        private bool DrivableIsActive = false;

        public CameraSelectorView()
        {
            Cameras.Add(new Camera("TV Set 1", "set1"));
            Cameras.Add(new Camera("TV Set 2", "set2"));
            Cameras.Add(new Camera("Helicam", "Helicam"));
            Cameras.Add(new Camera("Pitlane", "pitlane"));
            OnboardCameras.Add(new Camera("Cockpit", "Onboard0"));
            OnboardCameras.Add(new Camera("Driver", "Onboard1"));
            OnboardCameras.Add(new Camera("Dashboard", "Onboard2"));
            OnboardCameras.Add(new Camera("Rear", "Onboard3"));
            DrivableCameras.Add(new Camera("Chase", "Chase"));
            DrivableCameras.Add(new Camera("Far Chase", "FarChase"));
            DrivableCameras.Add(new Camera("Bonnet", "Bonnet"));
            DrivableCameras.Add(new Camera("Dash Pro", "DashPro"));
            DrivableCameras.Add(new Camera("Cockpit", "Cockpit"));
            DrivableCameras.Add(new Camera("Dash", "Dash"));
            DrivableCameras.Add(new Camera("Helmet", "Helmet"));
            ACCService.Client.MessageHandler.OnRealtimeUpdate += OnRealtimeUpdate;
            this.InitializeComponent();
        }

        void OnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Camera camera = Cameras.FirstOrDefault(x => x.InternalName == button.Name);
            if (camera != null)
            {
                ACCService.Client.MessageHandler.SetCamera(camera.InternalName, "default");
            }
        }

        private void OnRealtimeUpdate(string sender, RealtimeUpdate update)
        {
            Camera currentCamera = Cameras.FirstOrDefault(x => x.IsActive);

            if (update.ActiveCameraSet == "Onboard")
            {
                IsOnboardActive(true);
                if (DrivableIsActive)
                {
                    IsDrivableActive(false);
                }
                if (currentCamera != null)
                {
                    currentCamera.SetActive(false);
                }
            }
            else if (update.ActiveCameraSet == "Drivable")
            {
                IsDrivableActive(true);
                if (OnboardIsActive)
                {
                    IsOnboardActive(false);
                }
                if (currentCamera != null)
                {
                    currentCamera.SetActive(false);
                }
            }
            else
            {
                if (currentCamera != null)
                {
                    if (currentCamera.InternalName != update.ActiveCameraSet)
                    {
                        currentCamera.SetActive(false);
                        Camera newCamera = Cameras.FirstOrDefault(x => x.InternalName == update.ActiveCameraSet);
                        newCamera.SetActive(true);
                        IsDrivableActive(false);
                        IsOnboardActive(false);
                    }
                } else
                {
                    Camera newCamera = Cameras.FirstOrDefault(x => x.InternalName == update.ActiveCameraSet);
                    newCamera.SetActive(true);
                    IsDrivableActive(false);
                    IsOnboardActive(false);
                }
            }
        }

        private void OnboardSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Camera camera = e.AddedItems[0] as Camera;
            ACCService.Client.MessageHandler.SetCamera("Onboard", camera.InternalName);
            Camera previousCamera = OnboardCameras.FirstOrDefault(x => x.IsActive == true);
            if (previousCamera != null)
            {
                previousCamera.IsActive = false;
            }
            camera.IsActive = true;
        }

        private void OnboardDropDownClosed(object sender, object e)
        {
            Camera camera = OnboardCameras.FirstOrDefault(x => x.IsActive == true);
            if (camera != null)
            {
                ACCService.Client.MessageHandler.SetCamera("Onboard", camera.InternalName);
            }
        }

        private void DrivableSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Camera camera = e.AddedItems[0] as Camera;
            ACCService.Client.MessageHandler.SetCamera("Drivable", camera.InternalName);
            Camera previousCamera = DrivableCameras.FirstOrDefault(x => x.IsActive == true);
            if (previousCamera != null)
            {
                previousCamera.IsActive = false;
            }
            camera.IsActive = true;
        }

        private void DrivableDropDownClosed(object sender, object e)
        {
            Camera camera = DrivableCameras.FirstOrDefault(x => x.IsActive == true);
            if (camera != null)
            {
                ACCService.Client.MessageHandler.SetCamera("Drivable", camera.InternalName);
            }
        }

        private void IsOnboardActive(bool IsActive)
        {
            if (IsActive)
            {
                OnboardIsActive = true;
                OnboardsComboBox.Background = new SolidColorBrush(Microsoft.UI.Colors.Red);
            } else
            {
                OnboardIsActive = false;
                OnboardsComboBox.Background = null;
            }
        }

        private void IsDrivableActive(bool IsActive)
        {
            if (IsActive)
            {
                DrivableIsActive = true;
                DrivableComboBox.Background = new SolidColorBrush(Microsoft.UI.Colors.Red);
            }
            else
            {
                DrivableIsActive = false;
                DrivableComboBox.Background = null;
            }
        }
    }
}
