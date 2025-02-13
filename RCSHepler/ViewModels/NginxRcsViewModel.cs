using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCSHepler.ViewModels
{
    public class NginxRcsViewModel : INotifyPropertyChanged
    {
        private string? _port;
        private string? _backendAddress;

        public string? BackendAddress
        {
            get => _backendAddress;
            set
            {
                _backendAddress = value;
                OnPropertyChanged(nameof(BackendAddress));
            }
        }

        public string? Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
