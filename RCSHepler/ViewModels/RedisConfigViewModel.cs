using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCSHepler.ViewModels
{
    public class RedisConfigViewModel : INotifyPropertyChanged
    {
        private string? password;
        private string? iPEndpoint;

        public string? IPEndpoint
        {
            get => iPEndpoint;
            set
            {
                iPEndpoint = value;
                OnPropertyChanged(nameof(IPEndpoint));
            }
        }

        public string? Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
