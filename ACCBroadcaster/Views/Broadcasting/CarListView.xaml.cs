using ACCBroadcaster.Classes;
using ksBroadcastingNetwork;
using ksBroadcastingNetwork.Structs;
using Microsoft.UI;
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
using System.Timers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ACCBroadcaster.Properties;

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
        private float CurrentSessionTime = 0;
        private List<Button> CarPositionButtons = new List<Button>();
        private Timer CheckDisconnectsTimer;

        public CarListView()
        {
            this.InitializeComponent();
            CreateCameras();
            CreateCarContextFlyout();
            ACCService.Client.MessageHandler.OnEntrylistUpdate += OnEntrylistUpdate;
            ACCService.Client.MessageHandler.OnRealtimeCarUpdate += OnRealtimeCarUpdate;
            ACCService.Client.MessageHandler.OnRealtimeUpdate += OnRealtimeUpdate;
            CheckDisconnectsTimer = new Timer(5000);
            CheckDisconnectsTimer.Elapsed += CheckDisconnects;
            CheckDisconnectsTimer.AutoReset = true;
            CheckDisconnectsTimer.Enabled = true;
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
                        DriverName = $"{carUpdate.Drivers[carUpdate.CurrentDriverIndex].FirstName} {carUpdate.Drivers[carUpdate.CurrentDriverIndex].LastName}",
                        Location = CarLocationEnum.Pitlane,
                        ShortName = carUpdate.Drivers[carUpdate.CurrentDriverIndex].ShortName,
                        LastUpdated = DateTime.Now
                    };
                    Cars.Add(car);
                    Button button = new Button();
                    button.Template = CarPositionTemplate;
                    button.CommandParameter = car.Index;
                    button.Content = car.ShortName;
                    Grid.Children.Add(button);
                    CarPositionButtons.Add(button);
                }
                else
                {
                    car.RaceNumber = carUpdate.RaceNumber;
                    car.DriverName = $"{carUpdate.Drivers[carUpdate.CurrentDriverIndex].FirstName} {carUpdate.Drivers[carUpdate.CurrentDriverIndex].LastName}";
                    car.Location = CarLocationEnum.Pitlane;
                    car.ShortName = carUpdate.Drivers[carUpdate.CurrentDriverIndex].ShortName;
                    car.LastUpdated = DateTime.Now;
                    Button button = CarPositionButtons.FirstOrDefault(x => (int)x.CommandParameter == car.Index);
                    button.Content = car.ShortName;
                }
            }
            else
            {
                car = new Car
                {
                    Index = carUpdate.CarIndex,
                    RaceNumber = carUpdate.RaceNumber,
                    DriverName = $"{carUpdate.Drivers[carUpdate.CurrentDriverIndex].FirstName} {carUpdate.Drivers[carUpdate.CurrentDriverIndex].LastName}",
                    Location = CarLocationEnum.Pitlane,
                    ShortName = carUpdate.Drivers[carUpdate.CurrentDriverIndex].ShortName,
                    LastUpdated = DateTime.Now
                };
                Cars.Add(car);
                Button button = new Button();
                button.Template = CarPositionTemplate;
                button.CommandParameter = car.Index;
                button.Content = car.ShortName;
                Grid.Children.Add(button);
                CarPositionButtons.Add(button);
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
                    SortCars();
                }
                if (car.Position != 1 && car.Position != 99)
                {
                    Car carAhead = Cars.FirstOrDefault(x => x.Position == (car.Position - 1));
                    if (carAhead != null)
                    {
                        if (SessionType != RaceSessionType.Race)
                        {
                            if (carUpdate.BestSessionLap.LaptimeMS > 0)
                            {
                                int lapDelta = carAhead.BestLapMS - car.BestLapMS;
                                car.SetInterval(lapDelta);
                            }
                            else
                            {
                                car.Interval = null;
                            }
                        }
                        else if (carAhead.SplinePosition > car.SplinePosition && carAhead.Lap == car.Lap)
                        {
                            float splineDistance = Math.Abs(carAhead.SplinePosition - car.SplinePosition);
                            float gapFrontMeters = splineDistance * ACCService.Client.MessageHandler.TrackMeters;
                            car.Interval = $"+{gapFrontMeters / car.Kmh * 3.6:F3}";
                        }
                        else
                        {
                            car.Interval = null;
                        }
                    }
                }
                else
                {
                    car.Interval = null;
                }
                MoveCarButton(car);
                car.LastUpdated = DateTime.Now;
                car.IsConnected = true;
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
                        SetAsFocusedCar(focusedCar, true);
                        SetAsFocusedCar(previousFocusedCar, false);
                    }
                }
                else
                {
                    SetAsFocusedCar(focusedCar, true);
                }
            }
            SessionType = update.SessionType;
            CurrentSessionTime = Convert.ToInt32(update.SessionTime.TotalMilliseconds);
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
            MenuFlyoutItem item = (MenuFlyoutItem)sender;
            Car car = (Car)item.DataContext;
            ACCService.Client.MessageHandler.SetFocus((UInt16)car.Index, item.Name, "default");
        }

        private void OnCarContextOnboardClicked(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = (MenuFlyoutItem)sender;
            Car car = (Car)item.DataContext;
            ACCService.Client.MessageHandler.SetFocus((UInt16)car.Index, "Onboard", item.Name);
        }

        private void OnCarContextDrivableClicked(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = (MenuFlyoutItem)sender;
            Car car = (Car)item.DataContext;
            ACCService.Client.MessageHandler.SetFocus((UInt16)car.Index, "Drivable", item.Name);
        }

        private void OnCarContextReplayClicked(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = (MenuFlyoutItem)sender;
            Car car = (Car)item.DataContext;
            float length;
            if (item.Name == "Custom")
            {
                length = Settings.Default.CustomReplayLength;
            }
            else
            {
                length = (float)Convert.ToDouble(item.CommandParameter);
            }
            float requestedStartTime = CurrentSessionTime - (length * 1000);
            ACCService.Client.MessageHandler.RequestInstantReplay(requestedStartTime, length * 1000.0f, car.Index);
        }

        private void MoveCarButton(Car car)
        {
            Button button = CarPositionButtons.FirstOrDefault(x => (int)x.CommandParameter == car.Index);
            if (button != null)
            {
                double position = car.SplinePosition * TrackPositionLine.ActualHeight * 2;
                position -= TrackPositionLine.ActualHeight;
                button.Margin = new Thickness(0, 0, 0, position);
                button.Visibility = Visibility.Visible;
            }
        }

        private void SetAsFocusedCar(Car car, bool isFocused)
        {
            car.SetAsFocusedCar(isFocused);
            Button button = CarPositionButtons.FirstOrDefault(x => (int)x.CommandParameter == car.Index);
            if (isFocused)
            {
                button.Template = FocusedCarPositionTemplate;
            }
            else
            {
                button.Template = CarPositionTemplate;
            }
        }

        private void CarPostionButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int carIndex = (int)button.CommandParameter;
            ACCService.Client.MessageHandler.SetFocus((UInt16)carIndex);
        }

        // Every 5 seconds, check if a car has not recieved updates for more than 5 seconds to detect disconnected cars
        private void CheckDisconnects(Object source, ElapsedEventArgs e)
        {
            foreach (Car car in Cars)
            {
                if (car.IsConnected && car.LastUpdated.AddSeconds(5) < DateTime.Now)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        car.IsConnected = false;
                        car.Position = 99;
                        car.Interval = null;
                        car.Location = CarLocationEnum.NONE;
                        car.LapDelta = null;
                        car.CurrentLap = null;
                        Button button = CarPositionButtons.FirstOrDefault(x => (int)x.CommandParameter == car.Index);
                        button.Visibility = Visibility.Collapsed;
                        SortCars();
                    });
                }
            }
        }

        private void SortCars()
        {
            Cars = new ObservableCollection<Car>(Cars.OrderBy(x => x.Position));
            foreach (Car listedCar in CarLV.Items)
            {
                // Destroy PropertyChanged on all instances in view list to stop memory clogging up
                listedCar.DestroyPropertyChanged();
            }
            CarLV.ItemsSource = Cars;
        }
    }
}
