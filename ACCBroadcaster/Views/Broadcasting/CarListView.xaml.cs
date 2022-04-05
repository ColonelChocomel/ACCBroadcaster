using ACCBroadcaster.Classes;
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
    public sealed partial class CarListView : Page
    {
        private ObservableCollection<Car> Cars = new ObservableCollection<Car>();
        private ObservableCollection<Camera> Cameras = new ObservableCollection<Camera>();
        private ObservableCollection<Camera> OnboardCameras = new ObservableCollection<Camera>();
        private ObservableCollection<Camera> DrivableCameras = new ObservableCollection<Camera>();
        private RaceSessionType SessionType;
        public CarListView()
        {
            this.InitializeComponent();
            CreateCameras();
            CreateCarContextFlyout();
            ACCService.Client.MessageHandler.OnEntrylistUpdate += OnEntrylistUpdate;
            ACCService.Client.MessageHandler.OnRealtimeCarUpdate += OnRealtimeCarUpdate;
            ACCService.Client.MessageHandler.OnRealtimeUpdate += OnRealtimeUpdate;
        }

        private void OnEntrylistUpdate(string sender, CarInfo carUpdate)
        {
            Car car;

            if (Cars.Count > 0)
            {
                car = Cars.FirstOrDefault(x => x.Index == carUpdate.CarIndex);
                if (car == null)
                {
                    car = new Car
                    {
                        Index = carUpdate.CarIndex,
                        RaceNumber = carUpdate.RaceNumber,
                        DriverName = carUpdate.Drivers[carUpdate.CurrentDriverIndex].FirstName + " " + carUpdate.Drivers[carUpdate.CurrentDriverIndex].LastName,
                        Location = CarLocationEnum.Pitlane,
                    };
                    Cars.Add(car);
                }
                else
                {
                    car.RaceNumber = carUpdate.RaceNumber;
                    car.DriverName = carUpdate.Drivers[carUpdate.CurrentDriverIndex].FirstName + " " + carUpdate.Drivers[carUpdate.CurrentDriverIndex].LastName;
                    car.Location = CarLocationEnum.Pitlane;
                }
            }
            else
            {
                car = new Car
                {
                    Index = carUpdate.CarIndex,
                    RaceNumber = carUpdate.RaceNumber,
                    DriverName = carUpdate.Drivers[carUpdate.CurrentDriverIndex].FirstName + " " + carUpdate.Drivers[carUpdate.CurrentDriverIndex].LastName,
                    Location = CarLocationEnum.Pitlane,
                };
                Cars.Add(car);
            }
        }

        private void OnRealtimeCarUpdate(string sender, RealtimeCarUpdate carUpdate)
        {
            Car car = Cars.FirstOrDefault(x => x.Index == carUpdate.CarIndex);
            if (car != null)
            {
                bool positionChanged = false;
                car.Index = carUpdate.CarIndex;
                if (car.Position != carUpdate.Position)
                {
                    car.Position = carUpdate.Position;
                    positionChanged = true;
                }
                car.Location = carUpdate.CarLocation;
                car.SetLapDelta(carUpdate.Delta);
                car.CurrentLap = car.LapTimeMsToReadable(carUpdate.CurrentLap.LaptimeMS);
                car.LastLap = car.LapTimeMsToReadable(carUpdate.LastLap.LaptimeMS);
                car.BestLap = car.LapTimeMsToReadable(carUpdate.BestSessionLap.LaptimeMS);
                if (carUpdate.BestSessionLap.LaptimeMS != null)
                {
                    car.BestLapMS = (int)carUpdate.BestSessionLap.LaptimeMS;
                }
                car.SplinePosition = carUpdate.SplinePosition;
                car.Kmh = carUpdate.Kmh;
                car.Lap = carUpdate.Laps;
                if (positionChanged)
                {
                    Cars = new ObservableCollection<Car>(Cars.OrderBy(x => x.Position));
                    foreach(Car listedCar in CarLV.Items)
                    {
                        // Destroy PropertyChanged on all instances in view list to stop memory clogging up
                        listedCar.DestroyPropertyChanged();
                    }
                    CarLV.ItemsSource = Cars;
                }
                if (car.Position != 1)
                {
                    Car carAhead = Cars.FirstOrDefault(x => x.Position == (car.Position - 1));
                    if (carAhead != null)
                    {
                        if (SessionType != RaceSessionType.Race)
                        {
                            if (car.BestLapMS > 0)
                            {
                                int lapDelta = carAhead.BestLapMS - car.BestLapMS;
                                car.SetPositionDelta(lapDelta);
                            } else
                            {
                                car.PositionDelta = null;
                            }
                        }
                        else if (carAhead.SplinePosition > car.SplinePosition && carAhead.Lap == car.Lap)
                        {
                            float splineDistance = Math.Abs(carAhead.SplinePosition - car.SplinePosition);
                            float gapFrontMeters = splineDistance * ACCService.Client.MessageHandler.TrackMeters;
                            car.PositionDelta = $"+{gapFrontMeters / car.Kmh * 3.6:F3}";
                        } else
                        {
                            car.PositionDelta = null;
                        }
                    }
                } else
                {
                    car.PositionDelta = null;
                }
            }
        }

        private void OnRealtimeUpdate(string sender, RealtimeUpdate update)
        {
            Car focusedCar = Cars.FirstOrDefault(x => x.Index == update.FocusedCarIndex);
            if (focusedCar != null)
            {
                Car previousFocusedCar = Cars.FirstOrDefault(x => x.IsFocused);
                if (previousFocusedCar != null)
                {
                    if (previousFocusedCar.Index != focusedCar.Index)
                    {
                        focusedCar.SetAsFocusedCar(true);
                        previousFocusedCar.SetAsFocusedCar(false);
                    }
                } else
                {
                    focusedCar.SetAsFocusedCar(true);
                }
            }
            SessionType = update.SessionType;
        }

        private void OnCarClicked(object sender, TappedRoutedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)e.OriginalSource;
            Car car = (Car)textBlock.DataContext;
            ACCService.Client.MessageHandler.SetFocus((UInt16)car.Index);
        }

        private void CreateCameras()
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
        }

        private void CreateCarContextFlyout()
        {
            foreach (Camera camera in Cameras)
            {
                MenuFlyoutItem item = new MenuFlyoutItem();
                item.Text = camera.DisplayName;
                item.Name = camera.InternalName;
                item.Click += OnCarContextClicked;
                CarContextFlyout.Items.Add(item);
            }

            foreach (Camera camera in OnboardCameras)
            {
                MenuFlyoutItem item = new MenuFlyoutItem();
                item.Text = camera.DisplayName;
                item.Name = camera.InternalName;
                item.Click += OnCarContextOnboardClicked;
                OnboardContextFlyout.Items.Add(item);
            }

            foreach (Camera camera in DrivableCameras)
            {
                MenuFlyoutItem item = new MenuFlyoutItem();
                item.Text = camera.DisplayName;
                item.Name = camera.InternalName;
                item.Click += OnCarContextDrivableClicked;
                DrivableContextFlyout.Items.Add(item);
            }
        }

        private void OnCarContextClicked(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = (MenuFlyoutItem)e.OriginalSource;
            Car car = (Car)item.DataContext;
            ACCService.Client.MessageHandler.SetFocus((UInt16)car.Index, item.Name, "default");
        }

        private void OnCarContextOnboardClicked(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = (MenuFlyoutItem)e.OriginalSource;
            Car car = (Car)item.DataContext;
            ACCService.Client.MessageHandler.SetFocus((UInt16)car.Index, "Onboard", item.Name);
        }

        private void OnCarContextDrivableClicked(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = (MenuFlyoutItem)e.OriginalSource;
            Car car = (Car)item.DataContext;
            ACCService.Client.MessageHandler.SetFocus((UInt16)car.Index, "Drivable", item.Name);
        }
    }
}
