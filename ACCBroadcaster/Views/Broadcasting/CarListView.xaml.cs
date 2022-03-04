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
        public CarListView()
        {
            this.InitializeComponent();
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
                car.LapDelta = car.DeltaMsToReadable(carUpdate.Delta);
                car.CurrentLap = car.LapTimeMsToReadable(carUpdate.CurrentLap.LaptimeMS);
                car.LastLap = car.LapTimeMsToReadable(carUpdate.LastLap.LaptimeMS);
                car.BestLap = car.LapTimeMsToReadable(carUpdate.BestSessionLap.LaptimeMS);
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
        }

        private void OnCarClicked(object sender, TappedRoutedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)e.OriginalSource;
            Car car = (Car)textBlock.DataContext;
            ACCService.Client.MessageHandler.SetFocus((UInt16)car.Index);
        }
    }
}
