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
        private List<Car> Cars;
        public CarListView()
        {
            this.InitializeComponent();
            this.Cars = new List<Car>();
            Car car = new Car();
            car.Position = 0;
            car.RaceNumber = 123;
            car.DriverName = "Driver Name";
            car.Location = CarLocationEnum.Track;
            car.LapDelta = "-0.123";
            car.BestLap = "1:23.456";
            car.CurrentLap = "1:23.456";
            car.LastLap = "1:23.456";
            this.Cars.Add(car);
            car = new Car();
            car.Position = 1;
            car.RaceNumber = 133;
            car.DriverName = "Driver not Name";
            car.Location = CarLocationEnum.Track;
            car.LapDelta = "-0.123";
            car.BestLap = "1:23.456";
            car.CurrentLap = "1:23.456";
            car.LastLap = "1:23.456";
            this.Cars.Add(car);
        }
    }
}
