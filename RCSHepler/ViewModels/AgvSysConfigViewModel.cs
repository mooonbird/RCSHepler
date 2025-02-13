using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCSHepler.ViewModels
{
    public class AgvSysConfigViewModel : INotifyPropertyChanged
    {
        private string? _gRPCAddress;
        private string? _scheduleAddress;

        public string? ScheduleAddress
        {
            get => _scheduleAddress;
            set
            {
                _scheduleAddress = value;
                OnPropertyChanged(nameof(ScheduleAddress));
            }
        }

        public string? GRPCAddress
        {
            get => _gRPCAddress;
            set
            {
                _gRPCAddress = value;
                OnPropertyChanged(nameof(GRPCAddress));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
