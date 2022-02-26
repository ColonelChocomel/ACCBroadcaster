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
        }

        private void OnEntrylistUpdate(string sender, CarInfo carUpdate)
        {
            Car car = new Car
            {
                Index = carUpdate.CarIndex,
                RaceNumber = carUpdate.RaceNumber,
                DriverName = carUpdate.Drivers[carUpdate.CurrentDriverIndex].FirstName + " " + carUpdate.Drivers[carUpdate.CurrentDriverIndex].LastName,
                Location = CarLocationEnum.Pitlane,
            };
            Cars.Add(car);
        }

        private void OnRealtimeCarUpdate(string sender, RealtimeCarUpdate carUpdate)
        {
            Car car = Cars.FirstOrDefault(x => x.Index == carUpdate.CarIndex);
            if (car != null)
            {
                car.Index = carUpdate.CarIndex;
                car.Position = carUpdate.Position;
                car.Location = carUpdate.CarLocation;
                car.LapDelta =car.DeltaMsToReadable(carUpdate.Delta);
                car.CurrentLap = car.LapTimeMsToReadable(carUpdate.CurrentLap.LaptimeMS);
                car.LastLap = car.LapTimeMsToReadable(carUpdate.LastLap.LaptimeMS);
                car.BestLap = car.LapTimeMsToReadable(carUpdate.BestSessionLap.LaptimeMS);
                Cars = new ObservableCollection<Car>(Cars.OrderBy(x => x.Position));
                CarLV.ItemsSource = Cars;
            }
        }
    }
}
