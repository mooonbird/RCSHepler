using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCSHepler.ViewModels
{
    public class AgvInterfaceViewModel : INotifyPropertyChanged
    {
        private string? _sysInterfaceTcp;
        private string? _sysInterfaceApi;

        public string? SysInterfaceApi
        {
            get => _sysInterfaceApi;
            set
            {
                _sysInterfaceApi = value;
                OnPropertyChanged(nameof(SysInterfaceApi));
            }
        }

        public string? SysInterfaceTcp
        {
            get => _sysInterfaceTcp;
            set
            {
                _sysInterfaceTcp = value;
                OnPropertyChanged(nameof(SysInterfaceTcp));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
