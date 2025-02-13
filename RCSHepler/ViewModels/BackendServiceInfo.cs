using RCSHepler.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ServiceType = RCSHepler.Services.ServiceType;

namespace RCSHepler.ViewModels
{
    public class BackendServiceInfo : INotifyPropertyChanged
    {
        private string? serviceName;
        private string? connectionStatus;
        private string? serviceStatus;

        public ServiceType ServiceType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string? ServiceName
        {
            get => serviceName; 

            set
            {
                serviceName = value;
                OnPropertyChanged(nameof(ServiceName));
            }
        }

        /// <summary>
        /// 连接状态
        /// </summary>
        public string? ConnectionStatus
        {
            get => connectionStatus; 
            set
            {
                connectionStatus = value;
                OnPropertyChanged(nameof(ConnectionStatus));
            }
        }

        /// <summary>
        /// 服务状态
        /// </summary>
        public string? ServiceStatus
        {
            get => serviceStatus; 
            set
            {
                serviceStatus = value;
                OnPropertyChanged(nameof(ServiceStatus));
                OnPropertyChanged(nameof(SolidBrush));
            }
        }

        public SolidColorBrush SolidBrush
        {
            get 
            {
                ServiceControllerStatus s = Enum.Parse<ServiceControllerStatus>(ServiceStatus!);

                return s switch
                {
                    ServiceControllerStatus.Stopped => new SolidColorBrush(Colors.Red),
                    ServiceControllerStatus.StartPending => new SolidColorBrush(Colors.YellowGreen),
                    ServiceControllerStatus.StopPending => new SolidColorBrush(Colors.YellowGreen),
                    ServiceControllerStatus.Running => new SolidColorBrush(Colors.Green),
                    ServiceControllerStatus.ContinuePending => new SolidColorBrush(Colors.YellowGreen),
                    ServiceControllerStatus.PausePending => new SolidColorBrush(Colors.YellowGreen),
                    ServiceControllerStatus.Paused => new SolidColorBrush(Colors.YellowGreen),
                    _ => new SolidColorBrush(Colors.Black),
                };
            }
            //set
            //{
            //    solidBrush = value;
            //    OnPropertyChanged(nameof(SolidBrush));
            //}
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
