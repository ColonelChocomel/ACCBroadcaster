using ksBroadcastingNetwork;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCBroadcaster.Classes
{
    internal class Car : INotifyPropertyChanged
    {
        public int Index { get; set; }
        private int _Position;
        public int Position 
        {
            get { return _Position; }
            set
            {
                _Position = value;
                OnPropertyChanged(nameof(Position));
            }
        }
        public int RaceNumber { get; set; }
        private string _DriverName;
        public string DriverName
        {
            get { return _DriverName; }
            set
            {
                _DriverName = value;
                OnPropertyChanged(nameof(DriverName));
            }
        }
        private CarLocationEnum _Location;
        public CarLocationEnum Location
        {
            get { return _Location; }
            set
            {
                _Location = value;
                OnPropertyChanged(nameof(Location));
            }
        }
        private string _LapDelta;
        public string LapDelta
        {
            get { return _LapDelta; }
            set
            {
                _LapDelta = value;
                OnPropertyChanged(nameof(LapDelta));
            }
        }
        private string _CurrentLap;
        public string CurrentLap
        {
            get { return _CurrentLap; }
            set
            {
                _CurrentLap = value;
                OnPropertyChanged(nameof(CurrentLap));
            }
        }
        private string _LastLap;
        public string LastLap
        {
            get { return _LastLap; }
            set
            {
                _LastLap = value;
                OnPropertyChanged(nameof(LastLap));
            }
        }
        private string _BestLap;
        public string BestLap
        {
            get { return _BestLap; }
            set
            {
                _BestLap = value;
                OnPropertyChanged(nameof(BestLap));
            }
        }

        private SolidColorBrush _ForegroundBrush = new SolidColorBrush(Microsoft.UI.Colors.White);
        public SolidColorBrush ForegroundBrush
        {
            get { return _ForegroundBrush; }
            set
            {
                _ForegroundBrush = value;
                OnPropertyChanged(nameof(ForegroundBrush));
            }
        }

        private SolidColorBrush _BackgroundBrush;
        public SolidColorBrush BackgroundBrush
        {
            get { return _BackgroundBrush; }
            set
            {
                _BackgroundBrush = value;
                OnPropertyChanged(nameof(BackgroundBrush));
            }
        }

        private bool _IsFocused = false;
        public bool IsFocused
        {
            get { return _IsFocused; }
            set
            {
                _IsFocused = value;
                OnPropertyChanged(nameof(IsFocused));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void DestroyPropertyChanged()
        {
            PropertyChanged = null;
        }

        public string LapTimeMsToReadable(int? laptimeMs)
        {
            if (laptimeMs == null)
                return "--";
            else
                return $"{TimeSpan.FromMilliseconds((double)laptimeMs):mm\\:ss\\.fff}";
        }

        public string DeltaMsToReadable(int deltaMs)
        {
            string posOrNeg;
            if (deltaMs < 0)
            {
                posOrNeg = "-";
            } else
            {
                posOrNeg = "+";
            }
            return (posOrNeg + $"{TimeSpan.FromMilliseconds(deltaMs):ss\\.fff}");
        }

        public void SetAsFocusedCar(bool isFocused)
        {
            IsFocused = isFocused;
            if (isFocused)
            {
                BackgroundBrush = new SolidColorBrush(Microsoft.UI.Colors.Red);
            } else
            {
                BackgroundBrush = null;
            }
        }
    }
}
