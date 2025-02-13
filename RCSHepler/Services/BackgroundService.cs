using Microsoft.Win32;
using MySqlX.XDevAPI.Common;
using RCSHepler.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RCSHepler.Services
{
    /// <summary>
    /// Windows服务
    /// </summary>
    public class BackgroundService
    {
        public readonly static string SysManager = ConfigurationManager.AppSettings["SysManager"]!;
        public readonly static string SysInterface = ConfigurationManager.AppSettings["SysInterface"]!;
        public readonly static string Dist = ConfigurationManager.AppSettings["dist"]!;

        public static void StopAll()
        {
            Stop(SysManager);

            Stop(SysInterface);

            Stop(Dist);
        }

        public static void StartAll()
        {
            Start(SysManager);

            Start(SysInterface);

            Start(Dist);
        }

        /// <summary>
        /// 加锁解决服务控制管理器 (SCM)的线程安全问题
        /// </summary>
        static readonly object _lock = new();

        public static ServiceController Start(string serviceName)
        {
            lock (_lock)
            {
                var service = new ServiceController(serviceName);

                try
                {
                    service.Refresh();

                    if (service.Status != ServiceControllerStatus.Stopped)
                        return service;

                    service.Start();

                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
                catch (Exception ex)
                {
                    Log.Information("@{exception}", ex);
                }

                return service;
            }
        }

        public static ServiceController Stop(string serviceName)
        {
            lock (_lock)
            {
                var service = new ServiceController(serviceName);

                try
                {
                    service.Refresh();

                    if (service.Status != ServiceControllerStatus.Running)
                        return service;

                    if (serviceName == Dist)
                        NginxService.Stop(serviceName);
                    else
                        service.Stop();

                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                catch (Exception ex)
                {
                    Log.Information("@{exception}", ex);
                }

                return service;
            }
        }

        public static ServiceControllerStatus GetStatus(string serviceName)
        {
            var service = new ServiceController(serviceName);

            service.Refresh();

            return service.Status;
        }

        public static void RefreshStatus(ObservableCollection<BackendServiceInfo> serviceInfos)
        {
            foreach (var item in serviceInfos)
            {
                item.ServiceStatus = GetStatus(item.ServiceName!).ToString();
            }
        }

        public static Dictionary<string, BackendServiceInfo> GetBackendServiceInfos()
        {
            Dictionary<string, BackendServiceInfo> serviceInfos = new();

            //数据服务
            var re = ConfigurationManager.AppSettings["SysManager"];

            serviceInfos.Add("SysManager", new BackendServiceInfo { ServiceName = re });

            return serviceInfos;
        }

    }
}
