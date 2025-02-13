using RCSHepler.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace RCSHepler.Services
{
    public class NginxService
    {
        public static string RootPath { get; set; } = RegisterService.GetDirectoryPath("dist");

        public static string DistConfigPath { get; set; } = Path.Combine(RootPath, @"dist\config.js");

        public static string NginxConfigPath { get; set; } = Path.Combine(RootPath, @"conf\nginx.conf");

        private static readonly string _distPattern = @"\s+baseUrl:\s*""(http:\/\/(\d+\.\d+\.\d+\.\d+:\d+)/)""";

        private static readonly string _nginxPattern = @"listen\s+(\d+);";

        public static void Stop(string serviceName)
        {
            var service = new ServiceController(serviceName);

            if (service.Status != ServiceControllerStatus.Running)
                return;

            var process = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "cmd.exe",
                Verb = "runas",
                Arguments = "/c " + "taskkill /f /t /im nginx-rcs.exe",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(process);

            service.WaitForStatus(ServiceControllerStatus.Stopped);
        }

        public static NginxRcsViewModel GetConfig() 
        {
            var result = new NginxRcsViewModel();

            //后端WebApi地址
            string distConfig = File.ReadAllText(DistConfigPath);

            result.BackendAddress = Regex.Match(distConfig, _distPattern).Groups[2].Value;

            //前端端口号
            string nginxConfig = File.ReadAllText(NginxConfigPath);

            result.Port = Regex.Match(nginxConfig, _nginxPattern).Groups[1].Value;

            return result;
        }

        public static void UpdateConfig(string address, string port)
        {
            var distConfig = File.ReadAllText(DistConfigPath);

            var regex = new Regex(_distPattern);

            var re = regex.Replace(distConfig, $@"{Environment.NewLine}     baseUrl: ""http://{address}/""", 1);

            var nginxConfig = File.ReadAllText(NginxConfigPath);

            var re2 = Regex.Replace(nginxConfig, _nginxPattern, $"listen {port};");
        }
    }
}
