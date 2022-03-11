using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCBroadcaster.Classes
{
    internal class HUDOption : INotifyPropertyChanged
    {
        public string DisplayName { get; set; }
        public string InternalName { get; set; }
        public bool IsActive { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public HUDOption(string displayName, string internalName)
        {
            DisplayName = displayName;
            InternalName = internalName;
            IsActive = false;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            if (isActive)
            {
                BackgroundBrush = new SolidColorBrush(Microsoft.UI.Colors.Red);
            }
            else
            {
                BackgroundBrush = null;
            }
        }
    }
}
