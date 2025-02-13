using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCSHepler.ViewModels
{
    public class SoftwareInfoViewModel : INotifyPropertyChanged
    {
        private string? _authorizationStatus;
        private string? _version;
        private string? _url;

        public string? Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged(nameof(Version));
            }
        }


        public string? Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }

        public string? AuthorizationStatus
        {
            get => _authorizationStatus;
            set
            {
                _authorizationStatus = value;
                OnPropertyChanged(nameof(AuthorizationStatus));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
