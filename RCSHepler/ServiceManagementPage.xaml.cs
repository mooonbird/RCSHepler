using MySql.Data.MySqlClient;
using RCSHepler.ConfigWindows;
using RCSHepler.ViewModels;
using RCSHepler.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
using Serilog;
using Google.Protobuf.Compiler;
using RCSHepler.VisualControllerFinder;

namespace RCSHepler
{
    /// <summary>
    /// SystemUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class ServiceManagementPage : Page
    {
        internal ObservableCollection<BackendServiceInfo> _schedulingServices = new();
        internal ObservableCollection<BackendServiceInfo> _databaseServices = new();

        public ServiceManagementPage()
        {
            InitializeComponent();

            InitializeDatabaseServices();

            InitializeSchedulingServices();

            //初始化服务保活定时器
            TimerService.KeepAliveTimer.Elapsed += (s, e) =>
            {
                try
                {
                    BackgroundService.StartAll();
                    BackgroundService.RefreshStatus(_schedulingServices);
                }
                catch (Exception ex)
                {
                    Log.Information("@{exception}", ex);
                }
            };

            TimerService.KeepAliveTimer.Start();

            TimerService.RefreshStatus(keepAliveButton);
        }

        private void InitializeSchedulingServices()
        {
            ServiceController[] services = ServiceController.GetServices();

            List<string?> displayNames = new()
            {
                ConfigurationManager.AppSettings["SysManager"],
                ConfigurationManager.AppSettings["SysInterface"],
                ConfigurationManager.AppSettings["dist"]
            };

            var schedulingServices = services.Where(s => displayNames.Contains(s.DisplayName))
                .Select(s => new BackendServiceInfo
                {
                    ServiceName = s.DisplayName,
                    ConnectionStatus = "/",
                    ServiceStatus = s.Status.ToString()
                }).ToList();

            _schedulingServices = new ObservableCollection<BackendServiceInfo>(schedulingServices);

            SchedulingServices.ItemsSource = _schedulingServices;
        }

        private void InitializeDatabaseServices()
        {
            List<string?> displayNames = new()
            {
                ConfigurationManager.AppSettings["DBServeName"],
                ConfigurationManager.AppSettings["RedisServerName"]
            };
            
            var databaseServices = ServiceController.GetServices()
                .Where(s => displayNames.Contains(s.DisplayName))
                .Select(s => new BackendServiceInfo
                {
                    ServiceName = s.DisplayName,
                    ConnectionStatus = "/",
                    ServiceStatus = s.Status.ToString()
                }).ToList();

            _databaseServices = new ObservableCollection<BackendServiceInfo>(databaseServices);

            DatabaseServices.ItemsSource = _databaseServices;
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Start(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BackendServiceInfo schedulingService)
            {
                var serviceName = schedulingService.ServiceName;
                var serviceStatus = BackgroundService.GetStatus(schedulingService.ServiceName!);

                if (serviceStatus != ServiceControllerStatus.Stopped)
                {
                    //MessageBox.Show($"【服务状态】{serviceStatus}");
                    return;
                }

                button.IsEnabled = false;

                try
                {
                    await Task.Run(() =>
                    {
                        BackgroundService.Start(schedulingService.ServiceName!);

                        schedulingService!.ServiceStatus = BackgroundService.GetStatus(serviceName!).ToString();
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                button.IsEnabled = true;
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Stop(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BackendServiceInfo schedulingService)
            {
                var serviceName = schedulingService.ServiceName;
                var serviceStatus = BackgroundService.GetStatus(schedulingService.ServiceName!);

                if (BackgroundService.GetStatus(serviceName!) != ServiceControllerStatus.Running)
                {
                    //MessageBox.Show($"【服务状态】{serviceStatus}");
                    return;
                }
                button.IsEnabled = false;

                try
                {
                    await Task.Run(() =>
                    {
                        BackgroundService.Stop(serviceName!);

                        schedulingService!.ServiceStatus = BackgroundService.GetStatus(serviceName!).ToString();
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                button.IsEnabled = true;
            }
        }

        /// <summary>
        /// 重启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Restart(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BackendServiceInfo schedulingService)
            {
                var serviceName = schedulingService.ServiceName;
                var serviceStatus = BackgroundService.GetStatus(serviceName!);

                if (BackgroundService.GetStatus(serviceName!) != ServiceControllerStatus.Running)
                {
                    //MessageBox.Show($"【服务状态】{serviceStatus}");
                    button.IsEnabled = true;
                    return;
                }

                button.IsEnabled = false;

                try
                {
                    await Task.Run(() =>
                    {
                        BackgroundService.Stop(serviceName!);

                        schedulingService!.ServiceStatus = BackgroundService.GetStatus(serviceName!).ToString();

                        BackgroundService.Start(serviceName!);

                        schedulingService.ServiceStatus = BackgroundService.GetStatus(serviceName!).ToString();
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("【重启失败】" + ex.Message);
                }

                button.IsEnabled = true;
            }
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Configure(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BackendServiceInfo schedulingService)
            {
                if (schedulingService.ServiceName == ConfigurationManager.AppSettings["SysManager"])
                {
                    new AgvSysManagerConfigWindow().ShowDialog();
                }

                if (schedulingService.ServiceName == ConfigurationManager.AppSettings["RedisServerName"])
                {
                    var redisConfig = new RedisConfigWindow(schedulingService.ConnectionStatus);

                    redisConfig.ShowDialog();

                    var service = _databaseServices.First(s => s.ServiceName == ConfigurationManager.AppSettings["RedisServerName"]);
                    service.ConnectionStatus = redisConfig.ConnectionStatus;
                }

                if (schedulingService.ServiceName == ConfigurationManager.AppSettings["SysInterface"])
                {
                    new AgvInterfaceConfigWindow().ShowDialog();
                }

                if (schedulingService.ServiceName == ConfigurationManager.AppSettings["dist"])
                {
                    new NginxRcsConfigWindow().ShowDialog();
                }
            }
        }

        /// <summary>
        /// 服务保活的启闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TimerService.KeepAliveTimer.Enabled)
            {
                TimerService.KeepAliveTimer.Stop();
            }
            else
            {
                TimerService.KeepAliveTimer.Start();
            }

            TimerService.RefreshStatus(keepAliveButton);
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load(object sender, RoutedEventArgs e)
        {
            TimerService.RefreshStatus(keepAliveButton);
            BackgroundService.RefreshStatus(_schedulingServices);
        }
    }
}
